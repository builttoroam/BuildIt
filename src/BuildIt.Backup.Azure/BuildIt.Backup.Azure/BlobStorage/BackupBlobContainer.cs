using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BuildIt.Backup.Azure.Operations;

namespace BuildIt.Backup.Azure.BlobStorage
{
    public class BackupBlobContainer
    {
        private const string MetadataCreatedTimeKey = "BackupContainerCreated";

        public static async Task InitiateContainerBackup(
            string sourceStorageAccountConnectionString,
            string targetStorageAccountConnectionString,
            string sourceContainerName,
            string targetContainerName,
            IBlobBackupNotifier notifier,
            TextWriter log = null)
        {
            // Todo - Error handling

            var sourceStorageAccount = CloudStorageAccount.Parse(sourceStorageAccountConnectionString);
            var targetStorageAccount = CloudStorageAccount.Parse(targetStorageAccountConnectionString);

            var sourceBlobClient = sourceStorageAccount.CreateCloudBlobClient();
            var targetBlobClient = targetStorageAccount.CreateCloudBlobClient();

            var sourceContainer = sourceBlobClient.GetContainerReference(sourceContainerName);
            var targetContainer = targetBlobClient.GetContainerReference(targetContainerName);
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
            var sourceSasToken = sourceContainer.GetSharedAccessSignature(new SharedAccessBlobPolicy());

            // Get a list of all blobs, might be worth moving this to a segmented/async manner
            // Todo - for the purposes of this exercise, we are only listing committed blobs, no snapshots
            var blobs = sourceContainer.ListBlobs(useFlatBlobListing: true);

            foreach (var listBlobItem in blobs)
            {
                var blobUri = listBlobItem.Uri;
                var blobName = Path.GetFileName(blobUri.ToString());
                log?.WriteLine($"Copying blob: {blobName}");

                // Only support block blobs for now, page blobs can't do snapshots which means we would have to acquire an infinite lease.
                var sourceBlob = listBlobItem as CloudBlockBlob;
                if (sourceBlob == null)
                {
                    throw new NullReferenceException($"Null Refernce on source blob, blobname: {blobName}");
                }

                var destBlob = targetContainer.GetBlockBlobReference(sourceBlob.Name);
                // create a snapshot of the source blob and use that for the copy operation to solve concurrency
                var blobSnapshot = await sourceBlob.CreateSnapshotAsync();

                var copyRef = await destBlob.StartCopyAsync(new Uri(blobSnapshot.SnapshotQualifiedStorageUri.PrimaryUri + sourceSasToken));
                // Might need to hold on to this returned ref string for something?
            }

            // Raise a message to say that all blobs have started their copy operation
            await notifier.NotifyBackupInitiated(sourceBlobClient.Credentials.AccountName,
                targetBlobClient.Credentials.AccountName, targetContainerName, sourceContainerName);
        }

        public static async Task<bool> MonitorBackupSinglePass(
            string targetStorageAccountConnectionString, 
            string targetContainerName,
            string sourceStorageAccountName,
            string sourceContainerName,
            IBlobBackupNotifier notifier, 
            TextWriter log = null)
        {
            var pendingCopy = false;

            var targetStorageAccount = CloudStorageAccount.Parse(targetStorageAccountConnectionString);
            var targetBlobClient = targetStorageAccount.CreateCloudBlobClient();
            var targetContainer = targetBlobClient.GetContainerReference(targetContainerName);

            // Get a list of all blobs including copy info, might be worth moving this to a segmented/async manner
            var destBlobList = targetContainer.ListBlobs(useFlatBlobListing: true, blobListingDetails: BlobListingDetails.Copy);

            foreach (var dest in destBlobList)
            {
                var destBlob = dest as CloudBlob;

                if (destBlob == null)
                {
                    throw new NullReferenceException($"Null Refernce on source blob, blobname: {dest.Uri}");
                }

                if (destBlob.CopyState.Status == CopyStatus.Aborted ||
                    destBlob.CopyState.Status == CopyStatus.Failed)
                {
                    // Todo - Pass an error to an appropriate service, something passed as a parameter to this method?
                    pendingCopy = true;
                    log?.WriteLine($"Copying blob failed for blob: {destBlob.Name} with copy state: {destBlob.CopyState}");
                    // restart the copy process?
                    await destBlob.StartCopyAsync(destBlob.CopyState.Source);
                }
                else if (destBlob.CopyState.Status == CopyStatus.Pending)
                {
                    pendingCopy = true;
                }

                // File has completed its copy operation
            }

            // Send a message to our notifier in case we are calling these methods via queue triggers, or some other implemented notifier interface
            await notifier.NotifyBackupProgress(sourceStorageAccountName, targetBlobClient.Credentials.AccountName, sourceContainerName, targetContainerName, !pendingCopy);
            return pendingCopy;
        }

        public static async Task FinaliseContainerBackup(
            string sourceStorageAccountConnectionString,
            string targetStorageAccountConnectionString,
            string targetContainerName,
            string sourceContainerName,
            int numberOfBackupsToRetain,
            IBlobBackupNotifier notifier,
            TextWriter log = null)
        {
            // Todo - error logging

            var sourceStorageAccount = CloudStorageAccount.Parse(sourceStorageAccountConnectionString);
            var targetStorageAccount = CloudStorageAccount.Parse(targetStorageAccountConnectionString);

            var sourceBlobClient = sourceStorageAccount.CreateCloudBlobClient();
            var targetBlobClient = targetStorageAccount.CreateCloudBlobClient();

            var sourceContainer = sourceBlobClient.GetContainerReference(sourceContainerName);

            // delete all snapshots from source container
            var blobs = sourceContainer.ListBlobs(useFlatBlobListing: true);
            foreach (var listBlobItem in blobs)
            {
                var blobUri = listBlobItem.Uri;
                var blobName = Path.GetFileName(blobUri.ToString());
                log?.WriteLine($"Deleting snapshots for blob: {blobName}");

                // Only support block blobs for now, page blobs can't do snapshots which means we would have to acquire an infinite lease.
                var sourceBlob = listBlobItem as CloudBlockBlob;
                if (sourceBlob == null)
                {
                    throw new NullReferenceException($"Null Refernce on source blob, blobname: {blobName}");
                }

                await sourceBlob.DeleteIfExistsAsync(DeleteSnapshotsOption.DeleteSnapshotsOnly, null, null, null);
            }

            if (numberOfBackupsToRetain > 0)
            {
                var containers = targetBlobClient.ListContainers(sourceContainerName, ContainerListingDetails.Metadata).ToList(); // Force enumeration so that we can sort
                if (containers.Count > numberOfBackupsToRetain)
                {
                    // Sort the containers by the date created tag in their metadata
                    var sortedContainers = containers.OrderBy(c => c.Metadata[MetadataCreatedTimeKey]);
                    var containersToDelete = sortedContainers.Take(containers.Count - numberOfBackupsToRetain);

                    foreach (var cloudBlobContainer in containersToDelete)
                    {
                        if (cloudBlobContainer.Name != targetContainerName)
                        {
                            log?.WriteLine($"Deleting backup container according to retention policy: {cloudBlobContainer.Name}");
                            await cloudBlobContainer.DeleteAsync();
                        }
                    }
                }
            }
        }
    }
}
