using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BuildIt.Backup.Azure.Operations
{
    public class BlobBackupQueueNotifier : IBlobBackupNotifier
    {
        private readonly string queueStorageAccountConnectionString;
        private readonly string queueName;

        private bool isInited;

        public BlobBackupQueueNotifier(string queueStorageAccountConnectionString, string queueName)
        {
            this.queueStorageAccountConnectionString = queueStorageAccountConnectionString;
            this.queueName = queueName;
        }

        public async Task Init()
        {
            var queueStorageAccount = CloudStorageAccount.Parse(queueStorageAccountConnectionString);
            var queueClient = queueStorageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();
            isInited = true;
        }

        public async Task NotifyBackupInitiated(
            string sourceStorageAccountName, 
            string targetStorageAccountName,
            string sourceContainerName, 
            string targetContainerName)
        {
            if (!isInited)
            {
                await Init();
            }

            var queueStorageAccount = CloudStorageAccount.Parse(queueStorageAccountConnectionString);
            var queueClient = queueStorageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(queueName);

            var notification = new BlobBackupOperationNotification(BlobBackupOperationType.BackupInitiated,
                sourceStorageAccountName, targetStorageAccountName, sourceContainerName, targetContainerName);
            var serialized = JsonConvert.SerializeObject(notification);
            var cloudMessage = new CloudQueueMessage(serialized);

            await queue.AddMessageAsync(cloudMessage);
        }
    }
}
