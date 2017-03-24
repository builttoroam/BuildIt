using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BuildIt.Backup.Azure.DACWebService;
using BuildIt.Backup.Azure.Operations;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BuildIt.Backup.Azure.SqlDb
{
    /// <summary>
    /// Contains methods that will initiate, monitor, and finalise a 
    /// backup of an Azure SQL Database to a target Cloud Storage account in an async manner.
    /// 
    /// This backup will result in .bacpac binary backups of Azure SQL Databases.
    /// </summary>
    public class BackupSqlDb
    {
        /// <summary>
        /// Starts the backup process by ensuring the Storage Container in the target storage account exists (or creates it if it does not),
        /// then triggers an async export command of the target database using the DAC Service to create a .bacpac of the database in Blob Storage.
        /// </summary>
        /// <param name="dbServerName">Public server endpoint, eg. [ServerName].database.windows.net</param>
        /// <param name="dbName">Name of the database that is being backed up</param>
        /// <param name="dbUsername">A valid admin username for the database to be backed up</param>
        /// <param name="dbPassword">The password for the user account that is being used to connect to the database</param>
        /// <param name="dbLocation">The Azure Resource location of the database server that is to be backed up</param>
        /// <param name="targetStorageAccountConnectionString">Connection String for the target storage account where the backup will be generated</param>
        /// <param name="targetContainerName">Name of the container that will be hold the backup blob</param>
        /// <param name="notifier">An implementation of IDbBackupNotifier that will inform on the progress or error of the backup process</param>
        /// <param name="log">An optional TraceWriter object for logging purposes</param>
        public static async Task InitiateDbBackup(
            string dbServerName,
            string dbName,
            string dbUsername,
            string dbPassword,
            AzureDbLocation dbLocation,
            string targetStorageAccountConnectionString,
            string targetContainerName,
            IDbBackupNotifier notifier,
            TraceWriter log = null)
        {
            CloudStorageAccount targetStorageAccount;
            if (!Helpers.ConnectStorageAccount(targetStorageAccountConnectionString, out targetStorageAccount, log))
            {
                await notifier.NotifyBackupError(dbServerName, dbName, null, Guid.Empty,
                    Constants.ErrorMessages.CannotConnectStorageAccount);
                log?.Error(Constants.ErrorMessages.CannotConnectStorageAccount);
                return;
            }

            var targetBlobClient = targetStorageAccount.CreateCloudBlobClient();
            var targetContainer = targetBlobClient.GetContainerReference(targetContainerName);

            await InitiateDbBackup(dbServerName, dbName, dbUsername, dbPassword, dbLocation, targetContainer, notifier, log);
        }

        /// <summary>
        /// Starts the backup process by ensuring the Storage Container in the target storage account exists (or creates it if it does not),
        /// then triggers an async export command of the target database using the DAC Service to create a .bacpac of the database in Blob Storage.
        /// </summary>
        /// <param name="dbServerName">Public server endpoint, eg. [ServerName].database.windows.net</param>
        /// <param name="dbName">Name of the database that is being backed up</param>
        /// <param name="dbUsername">A valid admin username for the database to be backed up</param>
        /// <param name="dbPassword">The password for the user account that is being used to connect to the database</param>
        /// <param name="dbLocation">The Azure Resource location of the database server that is to be backed up</param>
        /// <param name="targetContainer">A container reference to the Storage Container that will hold the backup blob</param>
        /// <param name="notifier">An implementation of IDbBackupNotifier that will inform on the progress or error of the backup process</param>
        /// <param name="log">An optional TraceWriter object for logging purposes</param>
        public static async Task InitiateDbBackup(
            string dbServerName,
            string dbName,
            string dbUsername,
            string dbPassword,
            AzureDbLocation dbLocation,
            CloudBlobContainer targetContainer,
            IDbBackupNotifier notifier,
            TraceWriter log = null)
        {
            await Helpers.EnsureContainerExists(targetContainer, log);

            var blobName = $"{dbName}/{DateTime.UtcNow:yyyy-MM-dd}/{DateTime.UtcNow:HH-mm-ss}/backup.bacpac";
            var credentials = new BlobStorageAccessKeyCredentials
            {
                Uri = $"{targetContainer.Uri}{blobName}",
                StorageAccessKey = targetContainer.ServiceClient.Credentials.ExportBase64EncodedKey()
            };

            var connectionInfo = new ConnectionInfo
            {
                ServerName = dbServerName,
                DatabaseName = dbName,
                UserName = dbUsername,
                Password = dbPassword
            };

            var export = new ExportInput
            {
                BlobCredentials = credentials,
                ConnectionInfo = connectionInfo
            };

            var request = WebRequest.Create($"{dbLocation.GetEndpointForRegion()}/Export");
            request.Method = "POST";
            request.ContentType = "application/xml";
            using (var stream = await request.GetRequestStreamAsync())
            {
                var dcs = new DataContractSerializer(typeof(ExportInput));
                dcs.WriteObject(stream, export);
            }

            try
            {
                log?.Info($"Initiating backup for SQL Database: {dbName}");
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new HttpException((int)response.StatusCode, response.StatusDescription);
                    }

                    // Use a notifier to send the operation id to a function that will check its status
                    using (var stream = response.GetResponseStream())
                    {
                        var dcs = new DataContractSerializer(typeof(Guid));
                        var operationId = (Guid)dcs.ReadObject(stream);
                        log?.Info($"Backup operation has been initiated with operation id: {operationId}");
                        await notifier.NotifyBackupInitiated(dbServerName, dbName, blobName, operationId);
                    }
                }
            }
            catch (Exception e)
            {
                var errorMessage =
                    $"Exception of type {e.GetType().Name} ocurred while backing up database: {dbName} \n" +
                    $"Exception details: {e}";
                log?.Error(errorMessage);
                await notifier.NotifyBackupError(dbServerName, dbName, blobName, Guid.Empty, errorMessage);
            }
        }

        public static async Task<bool> MonitorBackupSinglePass(
            string dbServerName,
            string dbName,
            string dbUsername,
            string dbPassword,
            AzureDbLocation dbLocation,
            string backupBlobName,
            Guid operationId,
            IDbBackupNotifier notifier,
            TraceWriter log = null)
        {
            var pendingCopy = false;

            var statusInput = new StatusInput
            {
                AuthenticationType = "SQL",
                ServerName = dbServerName,
                UserName = dbUsername,
                Password = dbPassword,
                RequestId = operationId.ToString("D")
            };

            var request = WebRequest.Create($"{dbLocation.GetEndpointForRegion()}/Status");
            request.Method = "POST";
            request.ContentType = "application/xml";
            using (var stream = await request.GetRequestStreamAsync())
            {
                var dcs = new DataContractSerializer(typeof(StatusInput));
                dcs.WriteObject(stream, statusInput);
            }

            try
            {
                log?.Info($"Checking status of backup operation {operationId} for database: {dbName}");
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new HttpException((int)response.StatusCode, response.StatusDescription);
                    }

                    using (var stream = response.GetResponseStream())
                    {
                        var dcs = new DataContractSerializer(typeof(StatusInfo[]));
                        var statusInfoArray = (StatusInfo[])dcs.ReadObject(stream);
                        var statusInfo = statusInfoArray?.FirstOrDefault(si => new Guid(si.RequestId) == operationId);
                        if (statusInfo != null)
                        {
                            if (statusInfo.Status == "Failed")
                            {
                                var errorMessage =
                                    $"Backing up database: {dbName} failed with an error message of: {statusInfo.ErrorMessage}";
                                log?.Error(errorMessage);
                                await notifier.NotifyBackupError(dbServerName, dbName, backupBlobName, operationId, errorMessage);
                            }
                            else if (statusInfo.Status.Contains("Running"))
                            {
                                log?.Info(
                                    $"Backup operation for databse: {dbName} is reporting status of: {statusInfo.Status}");
                                await notifier.NotifyBackupProgress(dbServerName, dbName, backupBlobName, operationId, true);
                                pendingCopy = true;
                            }
                            else if (statusInfo.Status == "Completed")
                            {
                                log?.Info($"Backup operation for database: {dbName} is complete.");
                                await notifier.NotifyBackupProgress(dbServerName, dbName, backupBlobName, operationId, false);
                            }
                        }
                        else
                        {
                            log?.Error($"Unable to obtain backup status information for database: {dbName} with operation id: {operationId}");
                            await notifier.NotifyBackupProgress(dbServerName, dbName, backupBlobName, operationId, true);
                            pendingCopy = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var errorMessage =
                    $"Exception of type {e.GetType().Name} ocurred while backing up database: {dbName} \n" +
                    $"Exception details: {e}";
                log?.Error(errorMessage);
                await notifier.NotifyBackupError(dbServerName, dbName, backupBlobName, operationId, errorMessage);
            }

            return pendingCopy;
        }

        public static async Task FinaliseDbBackup(
            string dbServerName,
            string dbName,
            string targetStorageAccountConnectionString,
            string targetContainerName,
            string backupBlobName,
            int numberOfBackupsToRetain,
            IDbBackupNotifier notifier,
            TraceWriter log = null,
            bool throwExceptionOnPageBlobs = false)
        {
            CloudStorageAccount targetStorageAccount;
            if (!Helpers.ConnectStorageAccount(targetStorageAccountConnectionString, out targetStorageAccount, log))
            {
                await notifier.NotifyBackupError(dbServerName, dbName, backupBlobName, Guid.Empty, Constants.ErrorMessages.CannotConnectStorageAccount);
                log?.Error(Constants.ErrorMessages.CannotConnectStorageAccount);
                return;
            }

            var targetBlobClient = targetStorageAccount.CreateCloudBlobClient();
            var targetContainer = targetBlobClient.GetContainerReference(targetContainerName);

            await FinaliseDbBackup(dbServerName, dbName, targetContainer, backupBlobName, numberOfBackupsToRetain, notifier, log, throwExceptionOnPageBlobs);
        }

        public static async Task FinaliseDbBackup(
            string dbServerName,
            string dbName,
            CloudBlobContainer targetContainer,
            string backupBlobName,
            int numberOfBackupsToRetain,
            IDbBackupNotifier notifier,
            TraceWriter log = null,
            bool throwExceptionOnPageBlobs = false)
        {
            if (numberOfBackupsToRetain > 0)
            {
                var blobs = targetContainer.ListBlobs(useFlatBlobListing: true, prefix: dbName).ToList(); // Force enumeration so we can sort and count
                if (blobs.Count > numberOfBackupsToRetain)
                {
                    var sortedBlobs = blobs.OrderBy(b => b.Uri.AbsolutePath); // Blob names are constructed from dates, this should be fine
                    var blobsToDelete = sortedBlobs.Take(blobs.Count - numberOfBackupsToRetain);

                    foreach (var listBlobItem in blobsToDelete)
                    {
                        var sourceBlob = listBlobItem as CloudBlockBlob;
                        if (sourceBlob == null)
                        {
                            log?.Warning($"Encountered unexpected Page blob with name: {listBlobItem.Uri}");
                            if (throwExceptionOnPageBlobs)
                            {
                                throw new NullReferenceException($"Null Refernce on source blob, blobname: {listBlobItem.Uri}");
                            }

                            continue;
                        }

                        if (sourceBlob.Name != backupBlobName && sourceBlob.Name.EndsWith(".bacpac"))
                        {
                            log?.Info($"Deleting Sql Db backup according to retention policy: {sourceBlob.Name}");
                            await sourceBlob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, null, null, null);
                        }
                    }
                }
            }

            log?.Info($"Backup of {dbName} to {targetContainer.Name} is complete and finalised.");
        }
    }
}
