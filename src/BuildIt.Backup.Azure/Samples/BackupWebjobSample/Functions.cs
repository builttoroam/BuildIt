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
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BackupWebjobSample
{
    public class Functions
    {
        // Note, for the purposes of this demo, this function is called on webjob startup, and is only called once
        // The function that initiates the backup process should always be a singleton, either explicitly, or via a singleton type trigger
        [NoAutomaticTrigger]
        [Singleton]
        public async Task InitiateBlobStorageBackup(TextWriter log)
        {
            var sourceStorageAccountConnectionString = ConfigurationManager.AppSettings["BackupSourceConnectionString"];
            var targetStorageAccountConnectionString = ConfigurationManager.AppSettings["BackupTargetConnectionString"];
            var sourceContainerName = ConfigurationManager.AppSettings["BackupSourceContainer"];
            var targetContainerName = sourceContainerName + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");

            var queueNotifier = new BlobBackupQueueNotifier(
                ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString,
                "blobbackuptest",
                TimeSpan.FromSeconds(30));

            log.WriteLine($"Initiating backup of container: {sourceContainerName}");

            await BackupBlobContainer.InitiateContainerBackup(
                sourceStorageAccountConnectionString,
                targetStorageAccountConnectionString,
                sourceContainerName,
                targetContainerName,
                queueNotifier,
                log);
        }

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called backuptest.
        public async Task ProcessBlobBackupQueueMessage(
            [QueueTrigger("blobbackuptest")] BlobBackupOperationNotification message,
            TextWriter log)
        {
            log.WriteLine($"Processing incomming blob container backup queue message: {JsonConvert.SerializeObject(message)}");

            var sourceStorageAccountConnectionString = ConfigurationManager.AppSettings["BackupSourceConnectionString"];
            var targetStorageAccountConnectionString = ConfigurationManager.AppSettings["BackupTargetConnectionString"];
            var sourceContainerName = ConfigurationManager.AppSettings["BackupSourceContainer"];
            int.TryParse(ConfigurationManager.AppSettings["NumberOfBackupsToRetain"], out var backupRetentionPolicy);

            if ((!sourceStorageAccountConnectionString.Contains(message.SourceStorageAccountName) ||
                !targetStorageAccountConnectionString.Contains(message.TargetStorageAccountName) ||
                !sourceContainerName.Equals(message.SourceContainerName)) && message.OperationType != BackupOperationType.Error)
            {
                throw new Exception("This message is not intended for this function");
            }

            var queueNotifier = new BlobBackupQueueNotifier(
                ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString,
                "backuptest",
                TimeSpan.FromSeconds(30));

            switch (message.OperationType)
            {
                case BackupOperationType.Initiated:
                    log.WriteLine("Received message that backup process has been initiated");
                    await BackupBlobContainer.MonitorBackupSinglePass(
                        targetStorageAccountConnectionString,
                        message.TargetContainerName,
                        message.SourceStorageAccountName,
                        sourceContainerName,
                        queueNotifier,
                        log);
                    break;
                case BackupOperationType.InProgress:
                    log.WriteLine("Received message that backup process is still in progress");
                    await BackupBlobContainer.MonitorBackupSinglePass(
                        targetStorageAccountConnectionString,
                        message.TargetContainerName,
                        message.SourceStorageAccountName,
                        sourceContainerName,
                        queueNotifier,
                        log);
                    break;
                case BackupOperationType.Complete:
                    log.WriteLine("Received message that backup process has been completed, running cleanup tasks");
                    await BackupBlobContainer.FinaliseContainerBackup(
                        sourceStorageAccountConnectionString,
                        targetStorageAccountConnectionString,
                        message.TargetContainerName,
                        sourceContainerName,
                        backupRetentionPolicy,
                        queueNotifier,
                        log);
                    break;
                case BackupOperationType.Error:
                    log.WriteLine("Backup has sent an error message, process may have been aborted, please check error message below:");
                    log.WriteLine(!string.IsNullOrEmpty(message.ErrorMessage)
                        ? message.ErrorMessage
                        : "No error message available.");
                    break;
                default:
                    break;
            }
        }

        [NoAutomaticTrigger]
        [Singleton]
        public async Task InitiateDatabaseStorageBackup(TextWriter log)
        {
            var serverName = ConfigurationManager.AppSettings["DbBackupServerName"];
            var dbName = ConfigurationManager.AppSettings["DbBackupDatabase"];
            var username = ConfigurationManager.AppSettings["DbBackupUsername"];
            var password = ConfigurationManager.AppSettings["DbBackupPassword"];
            var targetStorageAccountConnectionString = ConfigurationManager.AppSettings["BackupTargetConnectionString"];
            var targetContainer = ConfigurationManager.AppSettings["DbBackupTargetContainer"];
            Enum.TryParse(ConfigurationManager.AppSettings["DbBackupRegion"], out AzureDbLocation dbRegion);

            var queueNotifier = new DbBackupQueueNotifier(
                ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString,
                "dbbackuptest",
                TimeSpan.FromSeconds(30));

            await BackupSqlDb.InitiateDbBackup(serverName, dbName, username, password, dbRegion,
                targetStorageAccountConnectionString, targetContainer, queueNotifier, log);
        }

        public async Task ProcessDbBackupQueueMessage(
            [QueueTrigger("dbbackuptest")] DbBackupOperationNotification message,
            TextWriter log)
        {
            log.WriteLine($"Processing incomming database backup queue message: {JsonConvert.SerializeObject(message)}");

            var serverName = ConfigurationManager.AppSettings["DbBackupServerName"];
            var dbName = ConfigurationManager.AppSettings["DbBackupDatabase"];
            var username = ConfigurationManager.AppSettings["DbBackupUsername"];
            var password = ConfigurationManager.AppSettings["DbBackupPassword"];
            Enum.TryParse<AzureDbLocation>(ConfigurationManager.AppSettings["DbBackupRegion"], out var dbRegion);
            int.TryParse(ConfigurationManager.AppSettings["NumberOfBackupsToRetain"], out var backupRetentionPolicy);
            var targetStorageAccountConnectionString = ConfigurationManager.AppSettings["BackupTargetConnectionString"];
            var targetContainer = ConfigurationManager.AppSettings["DbBackupTargetContainer"];

            if ((serverName != message.DbServer ||
                 dbName != message.DbName) &&
                message.OperationType != BackupOperationType.Error)
            {
                throw new Exception("This message is not intended for this function");
            }

            var queueNotifier = new DbBackupQueueNotifier(
                ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString,
                "dbbackuptest",
                TimeSpan.FromSeconds(30));

            switch (message.OperationType)
            {
                case BackupOperationType.Initiated:
                    log.WriteLine("Received message that backup process has been initiated");
                    await BackupSqlDb.MonitorBackupSinglePass(
                        serverName,
                        dbName,
                        username,
                        password,
                        dbRegion,
                        message.BackupBlobName,
                        message.OperationId,
                        queueNotifier,
                        log);
                    break;
                case BackupOperationType.InProgress:
                    log.WriteLine("Received message that backup process is still in progress");
                    await BackupSqlDb.MonitorBackupSinglePass(
                        serverName,
                        dbName,
                        username,
                        password,
                        dbRegion,
                        message.BackupBlobName,
                        message.OperationId,
                        queueNotifier,
                        log);
                    break;
                case BackupOperationType.Complete:
                    log.WriteLine("Received message that database backup has been completed, running cleanup tasks");
                    await BackupSqlDb.FinaliseDbBackup(
                        serverName,
                        dbName,
                        targetStorageAccountConnectionString,
                        targetContainer,
                        message.BackupBlobName,
                        backupRetentionPolicy,
                        queueNotifier,
                        log);
                    break;
                case BackupOperationType.Error:
                    log.WriteLine("Backup has sent an error message, process may have been aborted, please check error message below:");
                    log.WriteLine(!string.IsNullOrEmpty(message.ErrorMessage)
                        ? message.ErrorMessage
                        : "No error message available.");
                    break;
                default:
                    break;
            }
        }
    }
}
