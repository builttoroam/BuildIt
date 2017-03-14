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
    public class BlobBackupQueueNotifier : BaseQueueOperationNotifier, IBlobBackupNotifier
    {
        public BlobBackupQueueNotifier(string queueStorageAccountConnectionString, string queueName, TimeSpan? inProgressNotificationDelay)
            : base(queueStorageAccountConnectionString, queueName, inProgressNotificationDelay)
        {
        }

        public async Task NotifyBackupInitiated(
            string sourceStorageAccountName,
            string targetStorageAccountName,
            string sourceContainerName,
            string targetContainerName)
        {
            var notification = new BlobBackupOperationNotification(
                BackupOperationType.Initiated,
                sourceStorageAccountName,
                targetStorageAccountName,
                sourceContainerName,
                targetContainerName,
                null);

            var serializedNotification = JsonConvert.SerializeObject(notification);
            await SendNotification(serializedNotification, false);
        }

        public async Task NotifyBackupProgress(
            string sourceStorageAccountName,
            string targetStorageAccountName,
            string sourceContainerName,
            string targetContainerName,
            bool backupInProgress)
        {
            var notification = new BlobBackupOperationNotification(
                backupInProgress ? BackupOperationType.InProgress : BackupOperationType.Complete,
                sourceStorageAccountName,
                targetStorageAccountName,
                sourceContainerName,
                targetContainerName,
                null);

            var serializedNotification = JsonConvert.SerializeObject(notification);
            await SendNotification(serializedNotification, backupInProgress);
        }

        public async Task NotifyBackupError(
            string sourceStorageAccountName,
            string targetStorageAccountName,
            string sourceContainerName,
            string targetContainerName,
            string errorMessage)
        {
            var notification = new BlobBackupOperationNotification(
                BackupOperationType.Error,
                sourceStorageAccountName,
                targetStorageAccountName,
                sourceContainerName,
                targetContainerName,
                errorMessage);

            var serializedNotification = JsonConvert.SerializeObject(notification);
            await SendNotification(serializedNotification, false);
        }
    }
}
