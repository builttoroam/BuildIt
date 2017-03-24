using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildIt.Backup.Azure.BlobStorage;
using BuildIt.Backup.Azure.Operations;
using BuildIt.Backup.Azure.SqlDb;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BackupWebjobSample
{
    public class Functions
    {
        private const string BackupTargetConnectionStringAppSetting = "BackupTargetConnectionString";
        private const string BlobBackupSourceConnectionStringAppSetting = "BackupSourceConnectionString";
        private const string BlobBackupSourceContainerAppSetting = "BackupSourceContainer";
        private const string DbBackupServerNameAppSetting = "DbBackupServerName";
        private const string DbBackupDatabaseAppSetting = "DbBackupDatabaase";
        private const string DbBackupUsernameAppSetting = "DbBackupUsername";
        private const string DbBackupPasswordAppSetting = "DbBackupPassword";
        private const string DbBackupTargetContainerAppSetting = "DbBackupTargetContainer";
        private const string DbBackupRegionAppSetting = "DbBackupRegion";
        private const string NumberOfBackupsToRetainAppSetting = "NumberOfBackupsToRetain";

        private const string BlobBackupQueueName = "blobbackuptest";
        private const string DbBackupQueueName = "dbbackuptest";

        private readonly IBlobBackupNotifier blobBackupNotifier;
        private readonly IDbBackupNotifier dbBackupNotifier;

        public Functions()
        {
            blobBackupNotifier = new BlobBackupQueueNotifier(
                ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString,
                BlobBackupQueueName,
                TimeSpan.FromSeconds(30));

            dbBackupNotifier = new DbBackupQueueNotifier(
                ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString,
                DbBackupQueueName,
                TimeSpan.FromSeconds(30));
        }

        // Note, for the purposes of this demo, this function is called on webjob startup, and is only called once
        // The function that initiates the backup process should always be a singleton, either explicitly, or via a singleton type trigger
        [NoAutomaticTrigger]
        [Singleton]
        public async Task InitiateBlobStorageBackup(TraceWriter log)
        {
            var sourceStorageAccountConnectionString = ConfigurationManager.AppSettings[BlobBackupSourceConnectionStringAppSetting];
            var targetStorageAccountConnectionString = ConfigurationManager.AppSettings[BackupTargetConnectionStringAppSetting];
            var sourceContainerName = ConfigurationManager.AppSettings[BlobBackupSourceContainerAppSetting];

            log.Info($"Initiating backup of container: {sourceContainerName}");

            await BackupBlobContainer.InitiateContainerBackup(
                sourceStorageAccountConnectionString,
                targetStorageAccountConnectionString,
                sourceContainerName,
                blobBackupNotifier,
                log);
        }

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called backuptest.
        public async Task ProcessBlobBackupQueueMessage(
            [QueueTrigger(BlobBackupQueueName)] BlobBackupOperationNotification message,
            TraceWriter log)
        {
            log.Info($"Processing incomming blob container backup queue message: {JsonConvert.SerializeObject(message)}");

            var sourceStorageAccountConnectionString = ConfigurationManager.AppSettings[BlobBackupSourceConnectionStringAppSetting];
            var targetStorageAccountConnectionString = ConfigurationManager.AppSettings[BackupTargetConnectionStringAppSetting];
            var sourceContainerName = ConfigurationManager.AppSettings[BlobBackupSourceContainerAppSetting];
            int.TryParse(ConfigurationManager.AppSettings[NumberOfBackupsToRetainAppSetting], out var backupRetentionPolicy);

            if ((!sourceStorageAccountConnectionString.Contains(message.SourceStorageAccountName) ||
                !targetStorageAccountConnectionString.Contains(message.TargetStorageAccountName) ||
                !sourceContainerName.Equals(message.SourceContainerName)) && message.OperationType != BackupOperationType.Error)
            {
                throw new Exception("This message is not intended for this function");
            }

            switch (message.OperationType)
            {
                case BackupOperationType.Initiated:
                    log.Info("Received message that backup process has been initiated");
                    await BackupBlobContainer.MonitorBackupSinglePass(
                        targetStorageAccountConnectionString,
                        message.TargetContainerName,
                        message.SourceStorageAccountName,
                        sourceContainerName,
                        blobBackupNotifier,
                        log);
                    break;
                case BackupOperationType.InProgress:
                    log.Info("Received message that backup process is still in progress");
                    await BackupBlobContainer.MonitorBackupSinglePass(
                        targetStorageAccountConnectionString,
                        message.TargetContainerName,
                        message.SourceStorageAccountName,
                        sourceContainerName,
                        blobBackupNotifier,
                        log);
                    break;
                case BackupOperationType.Complete:
                    log.Info("Received message that backup process has been completed, running cleanup tasks");
                    await BackupBlobContainer.FinaliseContainerBackup(
                        sourceStorageAccountConnectionString,
                        targetStorageAccountConnectionString,
                        sourceContainerName,
                        message.TargetContainerName,
                        backupRetentionPolicy,
                        blobBackupNotifier,
                        log);
                    break;
                case BackupOperationType.Error:
                    log.Error("Backup has sent an error message, process may have been aborted, please check error message below:");
                    log.Error(!string.IsNullOrEmpty(message.ErrorMessage)
                        ? message.ErrorMessage
                        : "No error message available.");
                    break;
                default:
                    break;
            }
        }

        [NoAutomaticTrigger]
        [Singleton]
        public async Task InitiateDatabaseStorageBackup(TraceWriter log)
        {
            var serverName = ConfigurationManager.AppSettings[DbBackupServerNameAppSetting];
            var dbName = ConfigurationManager.AppSettings[DbBackupDatabaseAppSetting];
            var username = ConfigurationManager.AppSettings[DbBackupUsernameAppSetting];
            var password = ConfigurationManager.AppSettings[DbBackupPasswordAppSetting];
            var targetStorageAccountConnectionString = ConfigurationManager.AppSettings[BackupTargetConnectionStringAppSetting];
            var targetContainer = ConfigurationManager.AppSettings[DbBackupTargetContainerAppSetting];
            Enum.TryParse<AzureDbLocation>(ConfigurationManager.AppSettings[DbBackupRegionAppSetting], out var dbRegion);

            await BackupSqlDb.InitiateDbBackup(serverName, dbName, username, password, dbRegion,
                targetStorageAccountConnectionString, targetContainer, dbBackupNotifier, log);
        }

        public async Task ProcessDbBackupQueueMessage(
            [QueueTrigger(DbBackupQueueName)] DbBackupOperationNotification message,
            TraceWriter log)
        {
            log.Info($"Processing incomming database backup queue message: {JsonConvert.SerializeObject(message)}");

            var serverName = ConfigurationManager.AppSettings[DbBackupServerNameAppSetting];
            var dbName = ConfigurationManager.AppSettings[DbBackupDatabaseAppSetting];
            var username = ConfigurationManager.AppSettings[DbBackupUsernameAppSetting];
            var password = ConfigurationManager.AppSettings[DbBackupPasswordAppSetting];
            Enum.TryParse<AzureDbLocation>(ConfigurationManager.AppSettings[DbBackupRegionAppSetting], out var dbRegion);
            int.TryParse(ConfigurationManager.AppSettings[NumberOfBackupsToRetainAppSetting], out var backupRetentionPolicy);
            var targetStorageAccountConnectionString = ConfigurationManager.AppSettings[BackupTargetConnectionStringAppSetting];
            var targetContainer = ConfigurationManager.AppSettings[DbBackupTargetContainerAppSetting];

            if ((serverName != message.DbServer ||
                 dbName != message.DbName) &&
                message.OperationType != BackupOperationType.Error)
            {
                throw new Exception("This message is not intended for this function");
            }

            switch (message.OperationType)
            {
                case BackupOperationType.Initiated:
                    log.Info("Received message that backup process has been initiated");
                    await BackupSqlDb.MonitorBackupSinglePass(
                        serverName,
                        dbName,
                        username,
                        password,
                        dbRegion,
                        message.BackupBlobName,
                        message.OperationId,
                        dbBackupNotifier,
                        log);
                    break;
                case BackupOperationType.InProgress:
                    log.Info("Received message that backup process is still in progress");
                    await BackupSqlDb.MonitorBackupSinglePass(
                        serverName,
                        dbName,
                        username,
                        password,
                        dbRegion,
                        message.BackupBlobName,
                        message.OperationId,
                        dbBackupNotifier,
                        log);
                    break;
                case BackupOperationType.Complete:
                    log.Info("Received message that database backup has been completed, running cleanup tasks");
                    await BackupSqlDb.FinaliseDbBackup(
                        serverName,
                        dbName,
                        targetStorageAccountConnectionString,
                        targetContainer,
                        message.BackupBlobName,
                        backupRetentionPolicy,
                        dbBackupNotifier,
                        log);
                    break;
                case BackupOperationType.Error:
                    log.Error("Backup has sent an error message, process may have been aborted, please check error message below:");
                    log.Error(!string.IsNullOrEmpty(message.ErrorMessage)
                        ? message.ErrorMessage
                        : "No error message available.");
                    break;
                default:
                    break;
            }
        }
    }
}
