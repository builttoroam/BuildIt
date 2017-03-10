using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;
using BuildIt.Backup.Azure.Operations;

namespace BuildIt.Backup.Azure.BlobStorage
{
    public class BackupBlobContainer
    {
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

        public static async Task<bool> MonitorBackupSinglePass(string targetStorageAccountConnectionString, string targetContainerName, TextWriter log = null)
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

                // Looks like everything has completed
            }

            return pendingCopy;
        }
    }
}
