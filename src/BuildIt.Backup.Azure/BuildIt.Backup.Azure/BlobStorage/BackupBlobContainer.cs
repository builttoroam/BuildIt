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
    /// <summary>
    /// Contains methods that will initiate, monitor, and finalise a 
    /// backup of a Cloud Blob Container to a target Cloud Storage account 
    /// in an async manner.
    /// 
    /// This backup process only supports Blob Blobs.
    /// </summary>
    public static class BackupBlobContainer
    {
        /// <summary>
        /// Starts the backup process by creating a Storage Container in the target storage account, creates a snapshot of every blob
        /// in the source container, then triggers and async copy command of that snapshot to the target container.
        /// </summary>
        /// <param name="sourceStorageAccountConnectionString">Connection String for the source storage account that owns the container to be backed up</param>
        /// <param name="targetStorageAccountConnectionString">Connection String for the target storage account where the backup will be generated</param>
        /// <param name="sourceContainerName">Name of the container to be backed up</param>
        /// <param name="notifier">An implementation of IBlobBackupNotifier that will inform on the progress or error of the backup process</param>
        /// <param name="log">An optional TraceWriter object for logging purposes</param>
        /// <param name="throwExceptionOnPageBlobs">If set to true, this method will throw an exception if it encounters Page Blobs in the source container</param>
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

            await InitiateContainerBackup(targetBlobClient, sourceContainer, notifier, log, throwExceptionOnPageBlobs);
        }

        /// <summary>
        /// Starts the backup process by creating a Storage Container in the target storage account, creates a snapshot of every blob
        /// in the source container, then triggers and async copy command of that snapshot to the target container.
        /// </summary>
        /// <param name="targetBlobClient">A validated Cloud Blob Client for the storage account where the backup will be generated</param>
        /// <param name="sourceContainer">A container reference to the Storage Container that will be backed up</param>
        /// <param name="notifier">An implementation of IBlobBackupNotifier that will inform on the progress or error of the backup process</param>
        /// <param name="log">An optional TraceWriter object for logging purposes</param>
        /// <param name="throwExceptionOnPageBlobs">If set to true, this method will throw an exception if it encounters Page Blobs in the source container</param>
        public static async Task InitiateContainerBackup(
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
            await notifier.NotifyBackupInitiated(sourceContainer.ServiceClient.Credentials.AccountName,
                targetBlobClient.Credentials.AccountName, sourceContainer.Name, targetContainer.Name);
        }

        /// <summary>
        /// Checks the progress of a backup operation by querying the copy state of every blob in the target container.
        /// Will raise an error notification via the notifier if any blob's status is 'aborted' or 'failed'.
        /// 
        /// Will then raise a progress notification once every blob has been queried, which will also state whether the copy is complete
        /// or still in progress.
        /// </summary>
        /// <param name="targetStorageAccountConnectionString">Connection String for the target storage account where the backup will be generated</param>
        /// <param name="targetContainerName">Name of the container which is receiving the backup</param>
        /// <param name="sourceStorageAccountName">The name of the storage account which owns the backup source container</param>
        /// <param name="sourceContainerName">Name of the container to be backed up</param>
        /// <param name="notifier">An implementation of IBlobBackupNotifier that will inform on the progress or error of the backup process</param>
        /// <param name="log">An optional TraceWriter object for logging purposes</param>
        /// <param name="throwExceptionOnPageBlobs">If set to true, this method will throw an exception if it encounters Page Blobs in the source container</param>
        /// <returns>Returns true if there are still blobs waiting to be copied, false if all copy operations are complete.</returns>
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

            return await MonitorBackupSinglePass(targetContainer, sourceStorageAccountName,
                sourceContainerName, notifier, log, throwExceptionOnPageBlobs);
        }

        /// <summary>
        /// Checks the progress of a backup operation by querying the copy state of every blob in the target container.
        /// Will raise an error notification via the notifier if any blob's status is 'aborted' or 'failed'.
        /// 
        /// Will then raise a progress notification once every blob has been queried, which will also state whether the copy is complete
        /// or still in progress.
        /// </summary>
        /// <param name="targetContainer">A container reference to the Storage Container which will receive the backup</param>
        /// <param name="sourceStorageAccountName">The name of the storage account which owns the backup source container</param>
        /// <param name="sourceContainerName">Name of the container to be backed up</param>
        /// <param name="notifier">An implementation of IBlobBackupNotifier that will inform on the progress or error of the backup process</param>
        /// <param name="log">An optional TraceWriter object for logging purposes</param>
        /// <param name="throwExceptionOnPageBlobs">If set to true, this method will throw an exception if it encounters Page Blobs in the source container</param>
        /// <returns>Returns true if there are still blobs waiting to be copied, false if all copy operations are complete.</returns>
        public static async Task<bool> MonitorBackupSinglePass(
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
                    await notifier.NotifyBackupError(sourceStorageAccountName, targetContainer.ServiceClient.Credentials.AccountName, sourceContainerName,
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
            await notifier.NotifyBackupProgress(sourceStorageAccountName, targetContainer.ServiceClient.Credentials.AccountName, sourceContainerName, targetContainer.Name, pendingCopy);
            return pendingCopy;
        }

        /// <summary>
        /// Checks the number of backup containers beginning with the prefix of the source container name, if there are more than the number of backups to retain
        /// it will delete the oldest ones until the count matches the number of backups to retain. Will also optionally delete the snapshots generated by
        /// the initialisation step from the source container.
        /// </summary>
        /// <param name="sourceStorageAccountConnectionString">Connection String for the source storage account that owns the container to be backed up</param>
        /// <param name="targetStorageAccountConnectionString">Connection String for the target storage account where the backup will be generated</param>
        /// <param name="sourceContainerName">Name of the container to be backed up</param>
        /// <param name="targetContainerName">Name of the container which is receiving the backup</param>
        /// <param name="numberOfBackupsToRetain">The number of historical backups to retain</param>
        /// <param name="notifier">An implementation of IBlobBackupNotifier that will inform on the progress or error of the backup process</param>
        /// <param name="log">An optional TraceWriter object for logging purposes</param>
        /// <param name="throwExceptionOnPageBlobs">If set to true, this method will throw an exception if it encounters Page Blobs in the source container</param>
        /// <param name="deleteSnapshotsFromCopySource">If set to false, will not delete snapshots generated by initialisation process</param>
        public static async Task FinaliseContainerBackup(
            string sourceStorageAccountConnectionString,
            string targetStorageAccountConnectionString,
            string sourceContainerName,
            string targetContainerName,
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
            var targetContainer = targetBlobClient.GetContainerReference(targetContainerName);

            await FinaliseContainerBackup(sourceContainer, targetContainer,
                numberOfBackupsToRetain, notifier, log, throwExceptionOnPageBlobs, deleteSnapshotsFromCopySource);
        }

        /// <summary>
        /// Checks the number of backup containers beginning with the prefix of the source container name, if there are more than the number of backups to retain
        /// it will delete the oldest ones until the count matches the number of backups to retain. Will also optionally delete the snapshots generated by
        /// the initialisation step from the source container.
        /// </summary>
        /// <param name="sourceContainer">A container reference to the Storage Container that will be backed up</param>
        /// <param name="targetContainer">A container reference to the Storage Container which will receive the backup</param>
        /// <param name="numberOfBackupsToRetain">The number of historical backups to retain</param>
        /// <param name="notifier">An implementation of IBlobBackupNotifier that will inform on the progress or error of the backup process</param>
        /// <param name="log">An optional TraceWriter object for logging purposes</param>
        /// <param name="throwExceptionOnPageBlobs">If set to true, this method will throw an exception if it encounters Page Blobs in the source container</param>
        /// <param name="deleteSnapshotsFromCopySource">If set to false, will not delete snapshots generated by initialisation process</param>
        public static async Task FinaliseContainerBackup(
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
                var containers = targetContainer.ServiceClient.ListContainers(sourceContainer.Name, ContainerListingDetails.Metadata).ToList(); // Force enumeration so that we can sort and count
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
