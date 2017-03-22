using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BuildIt.Backup.Azure.Operations;
using Microsoft.Azure.WebJobs.Host;

namespace BuildIt.Backup.Azure.BlobStorage
{
    public class BackupBlobContainer
    {
        public static async Task InitiateContainerBackup(
            string sourceStorageAccountConnectionString,
            string targetStorageAccountConnectionString,
            string sourceContainerName,
            IBlobBackupNotifier notifier,
            TraceWriter log = null,
            bool throwExceptionOnPageBlobs = false)
        {

            CloudStorageAccount sourceStorageAccount;
            CloudStorageAccount targetStorageAccount;
            if (!Helpers.ConnectStorageAccount(sourceStorageAccountConnectionString, out sourceStorageAccount, log) ||
                !Helpers.ConnectStorageAccount(targetStorageAccountConnectionString, out targetStorageAccount, log))
            {
                await notifier.NotifyBackupError(null, null, sourceContainerName, null, Constants.ErrorMessages.CannotConnectMultipleContainers);
                log?.Error(Constants.ErrorMessages.CannotConnectMultipleContainers);
                return;
            }

            var sourceBlobClient = sourceStorageAccount.CreateCloudBlobClient();
            var targetBlobClient = targetStorageAccount.CreateCloudBlobClient();

            var sourceContainer = sourceBlobClient.GetContainerReference(sourceContainerName);

            await InitiateContainerBackup(sourceBlobClient, targetBlobClient, sourceContainer, notifier, log, throwExceptionOnPageBlobs);
        }

        public static async Task InitiateContainerBackup(
            CloudBlobClient sourceBlobClient,
            CloudBlobClient targetBlobClient,
            CloudBlobContainer sourceContainer,
            IBlobBackupNotifier notifier,
            TraceWriter log = null,
            bool throwExceptionOnPageBlobs = false)
        {
            var targetContainerName = sourceContainer.Name + DateTime.UtcNow.ToString(Constants.DateFormats.BlobBackupContainerName);
            var targetContainer = targetBlobClient.GetContainerReference(targetContainerName);
            await Helpers.EnsureContainerExists(targetContainer, log);

            // Set the target containers metadata so we can easily sort it when finalising
            targetContainer.Metadata[Constants.Metadata.ContainerCreatedTimeKey] = DateTime.UtcNow.ToString("s");
            await targetContainer.SetMetadataAsync();

            // Generate a shared access token for the copy operation
            var sourceSasToken = sourceContainer.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List,
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24)
            }, null);

            // Trim the leading '?' of the access token as we are goint to use snapshot URL's
            sourceSasToken = sourceSasToken.TrimStart('?');

            // Get a list of all blobs, might be worth moving this to a segmented/async manner
            // We are only backing up committed blobs, no snapshots
            var blobs = sourceContainer.ListBlobs(useFlatBlobListing: true);

            foreach (var listBlobItem in blobs)
            {
                try
                {
                    var blobUri = listBlobItem.Uri;
                    var blobName = Path.GetFileName(blobUri.ToString());
                    log?.Verbose($"Copying blob: {blobName}");

                    // Only support block blobs for now, page blobs can't do snapshots which means we would have to acquire an infinite lease.
                    var sourceBlob = listBlobItem as CloudBlockBlob;
                    if (sourceBlob == null)
                    {
                        log?.Warning($"Encountered unexpected Page blob with name: {blobName}");
                        if (throwExceptionOnPageBlobs)
                        {
                            throw new NullReferenceException($"Null Refernce on source blob, blobname: {blobName}");
                        }

                        continue;
                    }

                    var destBlob = targetContainer.GetBlockBlobReference(sourceBlob.Name);
                    // create a snapshot of the source blob and use that for the copy operation to solve concurrency
                    var blobSnapshot = await sourceBlob.CreateSnapshotAsync();

                    var copyUri = $"{blobSnapshot.SnapshotQualifiedStorageUri.PrimaryUri}&{sourceSasToken}";
                    var copyRef = await destBlob.StartCopyAsync(new Uri(copyUri));
                    log?.Info($"Copy operation for {blobName} intiated with reference code: {copyRef}");
                }
                catch (Exception e)
                {
                    var errorMessage =
                        $"Exception of type {e.GetType().Name} occurred while copying blob: {listBlobItem.Uri} \n" +
                        $"Exception message: {e.Message} \n" +
                        $"Exception Stacktrace: {e.StackTrace}";

                    await notifier.NotifyBackupError(targetBlobClient.Credentials.AccountName,
                        targetBlobClient.Credentials.AccountName, sourceContainer.Name, targetContainer.Name, errorMessage);
                    log?.Error(errorMessage);
                }
            }

            log?.Info($"Initiated copy operation on blobs from {sourceContainer.Name} to {targetContainer.Name}");

            // Raise a message to say that all blobs have started their copy operation
            await notifier.NotifyBackupInitiated(sourceBlobClient.Credentials.AccountName,
                targetBlobClient.Credentials.AccountName, sourceContainer.Name, targetContainer.Name);
        }

        public static async Task<bool> MonitorBackupSinglePass(
            string targetStorageAccountConnectionString,
            string targetContainerName,
            string sourceStorageAccountName,
            string sourceContainerName,
            IBlobBackupNotifier notifier,
            TraceWriter log = null,
            bool throwExceptionOnPageBlobs = false)
        {

            CloudStorageAccount targetStorageAccount;
            if (!Helpers.ConnectStorageAccount(targetStorageAccountConnectionString, out targetStorageAccount, log))
            {
                await notifier.NotifyBackupError(null, null, sourceContainerName, targetContainerName, Constants.ErrorMessages.CannotConnectStorageAccount);
                log?.Error(Constants.ErrorMessages.CannotConnectStorageAccount);
                return false;
            }

            var targetBlobClient = targetStorageAccount.CreateCloudBlobClient();
            var targetContainer = targetBlobClient.GetContainerReference(targetContainerName);

            return await MonitorBackupSinglePass(targetBlobClient, targetContainer, sourceStorageAccountName,
                sourceContainerName, notifier, log, throwExceptionOnPageBlobs);
        }

        public static async Task<bool> MonitorBackupSinglePass(
            CloudBlobClient targetBlobClient,
            CloudBlobContainer targetContainer,
            string sourceStorageAccountName,
            string sourceContainerName,
            IBlobBackupNotifier notifier,
            TraceWriter log = null,
            bool throwExceptionOnPageBlobs = false)
        {
            var pendingCopy = false;

            // Get a list of all blobs including copy info, might be worth moving this to a segmented/async manner
            var destBlobList = targetContainer.ListBlobs(useFlatBlobListing: true, blobListingDetails: BlobListingDetails.Copy);

            foreach (var dest in destBlobList)
            {
                var destBlob = dest as CloudBlob;

                if (destBlob == null)
                {
                    log?.Warning($"Encountered unexpected Page blob with name: {dest.Uri}");
                    if (throwExceptionOnPageBlobs)
                    {
                        throw new NullReferenceException($"Null Refernce on source blob, blobname: {dest.Uri}");
                    }

                    continue;
                }

                if (destBlob.CopyState.Status == CopyStatus.Aborted ||
                    destBlob.CopyState.Status == CopyStatus.Failed)
                {
                    // Pass an error to an appropriate service, using the notifier
                    var errorMessage =
                        $"Copying blob failed for blob: {destBlob.Name} with copy state: {destBlob.CopyState}. Copy operation will be restarted.";
                    await notifier.NotifyBackupError(sourceStorageAccountName, targetBlobClient.Credentials.AccountName, sourceContainerName,
                        targetContainer.Name, errorMessage);
                    log?.Error(errorMessage);
                }
                else if (destBlob.CopyState.Status == CopyStatus.Pending)
                {
                    pendingCopy = true;
                }

                // File has completed its copy operation
            }

            log?.Info($"Completed check of copy operation on container: {targetContainer.Name}. Blobs still pending: {pendingCopy}");
            // Send a message to our notifier in case we are calling these methods via queue triggers, or some other implemented notifier interface
            await notifier.NotifyBackupProgress(sourceStorageAccountName, targetBlobClient.Credentials.AccountName, sourceContainerName, targetContainer.Name, pendingCopy);
            return pendingCopy;
        }

        public static async Task FinaliseContainerBackup(
            string sourceStorageAccountConnectionString,
            string targetStorageAccountConnectionString,
            string targetContainerName,
            string sourceContainerName,
            int numberOfBackupsToRetain,
            IBlobBackupNotifier notifier,
            TraceWriter log = null,
            bool throwExceptionOnPageBlobs = false,
            bool deleteSnapshotsFromCopySource = true)
        {
            CloudStorageAccount sourceStorageAccount;
            CloudStorageAccount targetStorageAccount;
            if (!Helpers.ConnectStorageAccount(sourceStorageAccountConnectionString, out sourceStorageAccount, log) ||
                !Helpers.ConnectStorageAccount(targetStorageAccountConnectionString, out targetStorageAccount, log))
            {
                await notifier.NotifyBackupError(null, null, sourceContainerName, targetContainerName, Constants.ErrorMessages.CannotConnectMultipleContainers);
                log?.Error(Constants.ErrorMessages.CannotConnectMultipleContainers);
                return;
            }

            var sourceBlobClient = sourceStorageAccount.CreateCloudBlobClient();
            var targetBlobClient = targetStorageAccount.CreateCloudBlobClient();

            var sourceContainer = sourceBlobClient.GetContainerReference(sourceContainerName);
            var targetContainer = sourceBlobClient.GetContainerReference(targetContainerName);

            await FinaliseContainerBackup(sourceBlobClient, targetBlobClient, sourceContainer, targetContainer,
                numberOfBackupsToRetain, notifier, log, throwExceptionOnPageBlobs, deleteSnapshotsFromCopySource);
        }

        public static async Task FinaliseContainerBackup(
            CloudBlobClient sourceBlobClient,
            CloudBlobClient targetBlobClient,
            CloudBlobContainer sourceContainer,
            CloudBlobContainer targetContainer,
            int numberOfBackupsToRetain,
            IBlobBackupNotifier notifier,
            TraceWriter log = null,
            bool throwExceptionOnPageBlobs = false,
            bool deleteSnapshotsFromCopySource = true)
        {
            if (deleteSnapshotsFromCopySource)
            {
                // delete all snapshots from source container
                var blobs = sourceContainer.ListBlobs(useFlatBlobListing: true);
                foreach (var listBlobItem in blobs)
                {
                    var blobUri = listBlobItem.Uri;
                    var blobName = Path.GetFileName(blobUri.ToString());
                    log?.Verbose($"Deleting snapshots for blob: {blobName}");

                    // Only support block blobs for now, page blobs can't do snapshots which means we would have to acquire an infinite lease.
                    var sourceBlob = listBlobItem as CloudBlockBlob;
                    if (sourceBlob == null)
                    {
                        log?.Warning($"Encountered unexpected Page blob with name: {blobName}");
                        if (throwExceptionOnPageBlobs)
                        {
                            throw new NullReferenceException($"Null Refernce on source blob, blobname: {blobName}");
                        }

                        continue;
                    }

                    await sourceBlob.DeleteIfExistsAsync(DeleteSnapshotsOption.DeleteSnapshotsOnly, null, null, null);
                }
            }

            if (numberOfBackupsToRetain > 0)
            {
                var containers = targetBlobClient.ListContainers(sourceContainer.Name, ContainerListingDetails.Metadata).ToList(); // Force enumeration so that we can sort and count
                if (containers.Count > numberOfBackupsToRetain)
                {
                    // Sort the containers by the date created tag in their metadata
                    var sortedContainers = containers.OrderBy(c => c.Metadata[Constants.Metadata.ContainerCreatedTimeKey]);
                    var containersToDelete = sortedContainers.Take(containers.Count - numberOfBackupsToRetain);

                    foreach (var cloudBlobContainer in containersToDelete)
                    {
                        if (cloudBlobContainer.Name != targetContainer.Name)
                        {
                            log?.Info($"Deleting backup container according to retention policy: {cloudBlobContainer.Name}");
                            await cloudBlobContainer.DeleteAsync();
                        }
                    }
                }
            }

            log?.Info($"Backup of {sourceContainer.Name} to {targetContainer.Name} is complete and finalised.");
        }
    }
}
