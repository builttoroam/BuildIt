using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BuildIt.Backup.Azure.Operations
{
    /// <summary>
    /// A standard implementation of the IDbBackupNotifier,
    /// this class sends notifications by adding Azure Queue Messages to the
    /// Storage account and queue name specified in the constructor
    /// </summary>
    public class DbBackupQueueNotifier : BaseQueueOperationNotifier, IDbBackupNotifier
    {
        public DbBackupQueueNotifier(string queueStorageAccountConnectionString, string queueName, TimeSpan? inProgressNotificationDelay)
            : base(queueStorageAccountConnectionString, queueName, inProgressNotificationDelay)
        {
        }

        public async Task NotifyBackupInitiated(
            string dbServer,
            string dbName,
            Guid operationId)
        {
            var notification = new DbBackupOperationNotification(
                BackupOperationType.Initiated,
                dbServer,
                dbName,
                operationId,
                null);

            var serializedNotification = JsonConvert.SerializeObject(notification);
            await SendNotification(serializedNotification, false);
        }

        public async Task NotifyBackupProgress(
            string dbServer,
            string dbName,
            Guid operationId,
            bool backupInProgress)
        {
            var notification = new DbBackupOperationNotification(
                backupInProgress ? BackupOperationType.InProgress : BackupOperationType.Complete,
                dbServer,
                dbName,
                operationId,
                null);

            var serializedNotification = JsonConvert.SerializeObject(notification);
            await SendNotification(serializedNotification, backupInProgress);
        }

        public async Task NotifyBackupError(
            string dbServer,
            string dbName,
            Guid operationId,
            string errorMessage)
        {
            var notification = new DbBackupOperationNotification(
                BackupOperationType.Error,
                dbServer,
                dbName,
                operationId,
                errorMessage);

            var serializedNotification = JsonConvert.SerializeObject(notification);
            await SendNotification(serializedNotification, false);
        }
    }
}
