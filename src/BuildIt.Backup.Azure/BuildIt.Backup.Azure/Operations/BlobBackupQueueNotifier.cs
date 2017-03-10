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
    /// <summary>
    /// A standard implementation of the IBlobBackupNotifier,
    /// this class sends notifications by adding Azure Queue Messages to the
    /// Storage account and queue name specified in the constructor
    /// </summary>
    public class BlobBackupQueueNotifier : IBlobBackupNotifier
    {
        private string QueueStorageAccountConnectionString { get; }
        private string QueueName { get; }
        private TimeSpan? InProgressNotificationDelay { get; }

        private bool isInited;
        private CloudQueue queue;

        public BlobBackupQueueNotifier(string queueStorageAccountConnectionString, string queueName, TimeSpan? inProgressNotificationDelay)
        {
            QueueStorageAccountConnectionString = queueStorageAccountConnectionString;
            QueueName = queueName;
            InProgressNotificationDelay = inProgressNotificationDelay;
        }

        public async Task Init()
        {
            var queueStorageAccount = CloudStorageAccount.Parse(QueueStorageAccountConnectionString);
            var queueClient = queueStorageAccount.CreateCloudQueueClient();
            queue = queueClient.GetQueueReference(QueueName);
            await queue.CreateIfNotExistsAsync();
            isInited = true;
        }

        public async Task NotifyBackupInitiated(
            string sourceStorageAccountName, 
            string targetStorageAccountName,
            string sourceContainerName, 
            string targetContainerName)
        {
            var notification = new BlobBackupOperationNotification(
                BlobBackupOperationType.Initiated,
                sourceStorageAccountName,
                targetStorageAccountName, 
                sourceContainerName, 
                targetContainerName);

            await SendNotification(notification);
        }

        public async Task NotifyBackupProgress(
            string sourceStorageAccountName,
            string targetStorageAccountName,
            string sourceContainerName,
            string targetContainerName,
            bool backupCompleted)
        {
            var notification = new BlobBackupOperationNotification(
                backupCompleted ? BlobBackupOperationType.Complete : BlobBackupOperationType.InProgress,
                sourceStorageAccountName,
                targetStorageAccountName,
                sourceContainerName,
                targetContainerName);

            await SendNotification(notification);
        }

        private async Task SendNotification(BlobBackupOperationNotification notification)
        {
            if (!isInited)
            {
                await Init();
            }

            var serialized = JsonConvert.SerializeObject(notification);
            var cloudMessage = new CloudQueueMessage(serialized);

            var initialiVisibilityDelay = notification.OperationType == BlobBackupOperationType.InProgress
                ? InProgressNotificationDelay
                : null;

            await queue.AddMessageAsync(cloudMessage, null, initialiVisibilityDelay, null, null);
        }
    }
}
