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
    public abstract class BaseQueueOperationNotifier
    {
        private string QueueStorageAccountConnectionString { get; }
        private string QueueName { get; }
        private TimeSpan? InProgressNotificationDelay { get; }

        private bool isInited;
        private CloudQueue queue;

        protected BaseQueueOperationNotifier(string queueStorageAccountConnectionString, string queueName, TimeSpan? inProgressNotificationDelay)
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

        protected async Task SendNotification(string serializedNotification, bool delayNotification)
        {
            if (!isInited)
            {
                await Init();
            }

            var cloudMessage = new CloudQueueMessage(serializedNotification);

            var initialiVisibilityDelay = delayNotification
                ? InProgressNotificationDelay
                : null;

            await queue.AddMessageAsync(cloudMessage, null, initialiVisibilityDelay, null, null);
        }
    }
}
