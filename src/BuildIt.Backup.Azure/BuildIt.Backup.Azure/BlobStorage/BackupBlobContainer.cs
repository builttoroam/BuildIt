using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BuildIt.Backup.Azure.Operations;
using Microsoft.Azure.WebJobs.Host;

namespace BuildIt.Backup.Azure.BlobStorage
{
    public class BackupBlobContainer
    {
        private const string MetadataCreatedTimeKey = "BackupContainerCreated";

        public static async Task InitiateContainerBackup(
            string sourceStorageAccountConnectionString,
            string targetStorageAccountConnectionString,
            string sourceContainerName,
            string targetContainerName, // todo - drop this and make targetcontainername from source prefix
            IBlobBackupNotifier notifier,
            TraceWriter log = null)
        {
            CloudStorageAccount sourceStorageAccount;
            CloudStorageAccount targetStorageAccount;
            try
            {
                sourceStorageAccount = CloudStorageAccount.Parse(sourceStorageAccountConnectionString);
                targetStorageAccount = CloudStorageAccount.Parse(targetStorageAccountConnectionString);
            }
            catch (Exception e)
            {
                var errorMessage =
                    "Unable to connect to either source or target storage container to initiate backup. Check passed connection string values to ensure they are valid. \n" +
                    $"Exception thrown was of type {e.GetType().Name} with message: {e.Message} \n" +
                    $"Stacktrace for exception was {e.StackTrace}";
                await notifier.NotifyBackupError(null, null, sourceContainerName, targetContainerName, errorMessage);
                log?.Error(errorMessage);
                return;
            }

            var sourceBlobClient = sourceStorageAccount.CreateCloudBlobClient();
            var targetBlobClient = targetStorageAccount.CreateCloudBlobClient();

            var sourceContainer = sourceBlobClient.GetContainerReference(sourceContainerName);
            var targetContainer = targetBlobClient.GetContainerReference(targetContainerName);
            log?.Info($"Creating backup container with name: {targetContainerName}");
            await targetContainer.CreateIfNotExistsAsync();
            await Task.Delay(5000);
            var containerExists = await targetContainer.ExistsAsync();
            var waitCount = 0;
            while (!containerExists)
            {
                await Task.Delay(5000);
                containerExists = await targetContainer.ExistsAsync();
                waitCount++;
                if (waitCount > 30) // 2.5 minutes, really should have been created by now...
                {
                    throw new Exception("Unable to verify backup container");
                }
            }

            targetContainer.Metadata[MetadataCreatedTimeKey] = DateTime.UtcNow.ToString("s");
            await targetContainer.SetMetadataAsync();
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
                        // todo - optional [throw exception on page blob] parameter, otherwise continue/log
                        throw new NullReferenceException($"Null Refernce on source blob, blobname: {blobName}");
                    }

                    var destBlob = targetContainer.GetBlockBlobReference(sourceBlob.Name);
                    // create a snapshot of the source blob and use that for the copy operation to solve concurrency
                    var blobSnapshot = await sourceBlob.CreateSnapshotAsync();

                    var copyUri = $"{blobSnapshot.SnapshotQualifiedStorageUri.PrimaryUri}&{sourceSasToken}";
                    var copyRef = await destBlob.StartCopyAsync(new Uri(copyUri));
                    // Todo - log out copy start success + copyRef
                    // Might need to hold on to this returned ref string for something?

                }
                catch (Exception e)
                {
                    var errorMessage =
                        $"Exception of type {e.GetType().Name} occurred while copying blob: {listBlobItem.Uri} \n" +
                        $"Exception message: {e.Message} \n" +
                        $"Exception Stacktrace: {e.StackTrace}";

                    await notifier.NotifyBackupError(sourceBlobClient.Credentials.AccountName,
                        targetBlobClient.Credentials.AccountName, sourceContainerName, targetContainerName, errorMessage);
                    log?.Error(errorMessage);
                }
            }

            log?.Info($"Initiated copy operation on blobs from {sourceContainerName} to {targetContainerName}");
            // Raise a message to say that all blobs have started their copy operation
            await notifier.NotifyBackupInitiated(sourceBlobClient.Credentials.AccountName,
                targetBlobClient.Credentials.AccountName, sourceContainerName, targetContainerName);
        }

        public static async Task<bool> MonitorBackupSinglePass(
            string targetStorageAccountConnectionString,
            string targetContainerName,
            string sourceStorageAccountName,
            string sourceContainerName,
            IBlobBackupNotifier notifier,
            TraceWriter log = null)
        {
            var pendingCopy = false;

            CloudStorageAccount targetStorageAccount;
            try
            {
                targetStorageAccount = CloudStorageAccount.Parse(targetStorageAccountConnectionString);
            }
            catch (Exception e)
            {
                var errorMessage =
                    "Unable to connect to target storage container to initiate backup. Check passed connection string values to ensure they are valid. \n" +
                    $"Exception thrown was of type {e.GetType().Name} with message: {e.Message} \n" +
                    $"Stacktrace for exception was {e.StackTrace}";
                await notifier.NotifyBackupError(null, null, sourceContainerName, targetContainerName, errorMessage);
                log?.Error(errorMessage);
                return false;
            }

            var targetBlobClient = targetStorageAccount.CreateCloudBlobClient();
            var targetContainer = targetBlobClient.GetContainerReference(targetContainerName);

            // Get a list of all blobs including copy info, might be worth moving this to a segmented/async manner
            var destBlobList = targetContainer.ListBlobs(useFlatBlobListing: true, blobListingDetails: BlobListingDetails.Copy);

            foreach (var dest in destBlobList)
            {
                var destBlob = dest as CloudBlob;

                if (destBlob == null)
                {
                    // todo - optional [throw exception on page blob] parameter, otherwise continue/log
                    throw new NullReferenceException($"Null Refernce on source blob, blobname: {dest.Uri}");
                }

                if (destBlob.CopyState.Status == CopyStatus.Aborted ||
                    destBlob.CopyState.Status == CopyStatus.Failed)
                {
                    // Pass an error to an appropriate service, using the notifier
                    //pendingCopy = true;
                    var errorMessage =
                        $"Copying blob failed for blob: {destBlob.Name} with copy state: {destBlob.CopyState}. Copy operation will be restarted.";
                    await notifier.NotifyBackupError(sourceStorageAccountName, targetContainerName, sourceContainerName,
                        targetContainerName, errorMessage);
                    log?.Error(errorMessage);
                    // restart the copy process
                    //await destBlob.StartCopyAsync(destBlob.CopyState.Source);
                }
                else if (destBlob.CopyState.Status == CopyStatus.Pending)
                {
                    pendingCopy = true;
                }

                // File has completed its copy operation
            }

            log?.Info($"Completed check of copy operation on container: {targetContainerName}. Blobs still pending: {pendingCopy}");
            // Send a message to our notifier in case we are calling these methods via queue triggers, or some other implemented notifier interface
            await notifier.NotifyBackupProgress(sourceStorageAccountName, targetBlobClient.Credentials.AccountName, sourceContainerName, targetContainerName, pendingCopy);
            return pendingCopy;
        }

        public static async Task FinaliseContainerBackup(
            string sourceStorageAccountConnectionString,
            string targetStorageAccountConnectionString,
            string targetContainerName,
            string sourceContainerName,
            int numberOfBackupsToRetain,
            IBlobBackupNotifier notifier,
            TraceWriter log = null)
        {
            CloudStorageAccount sourceStorageAccount;
            CloudStorageAccount targetStorageAccount;
            try
            {
                sourceStorageAccount = CloudStorageAccount.Parse(sourceStorageAccountConnectionString);
                targetStorageAccount = CloudStorageAccount.Parse(targetStorageAccountConnectionString);
            }
            catch (Exception e)
            {
                var errorMessage =
                    "Unable to connect to either source or target storage container to initiate backup. Check passed connection string values to ensure they are valid. \n" +
                    $"Exception thrown was of type {e.GetType().Name} with message: {e.Message} \n" +
                    $"Stacktrace for exception was {e.StackTrace}";
                await notifier.NotifyBackupError(null, null, sourceContainerName, targetContainerName, errorMessage);
                log?.Error(errorMessage);
                return;
            }

            var sourceBlobClient = sourceStorageAccount.CreateCloudBlobClient();
            var targetBlobClient = targetStorageAccount.CreateCloudBlobClient();

            var sourceContainer = sourceBlobClient.GetContainerReference(sourceContainerName);

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
                    throw new NullReferenceException($"Null Refernce on source blob, blobname: {blobName}");
                }

                // Todo - optional parameter in case people dont want to delete snapshots
                await sourceBlob.DeleteIfExistsAsync(DeleteSnapshotsOption.DeleteSnapshotsOnly, null, null, null);
            }

            if (numberOfBackupsToRetain > 0)
            {
                var containers = targetBlobClient.ListContainers(sourceContainerName, ContainerListingDetails.Metadata).ToList(); // Force enumeration so that we can sort and count
                if (containers.Count > numberOfBackupsToRetain)
                {
                    // Sort the containers by the date created tag in their metadata
                    var sortedContainers = containers.OrderBy(c => c.Metadata[MetadataCreatedTimeKey]);
                    var containersToDelete = sortedContainers.Take(containers.Count - numberOfBackupsToRetain);

                    foreach (var cloudBlobContainer in containersToDelete)
                    {
                        if (cloudBlobContainer.Name != targetContainerName)
                        {
                            log?.Info($"Deleting backup container according to retention policy: {cloudBlobContainer.Name}");
                            await cloudBlobContainer.DeleteAsync();
                        }
                    }
                }
            }

            log?.Info($"Backup of {sourceContainerName} to {targetContainerName} is complete and finalised.");
        }
    }
}
