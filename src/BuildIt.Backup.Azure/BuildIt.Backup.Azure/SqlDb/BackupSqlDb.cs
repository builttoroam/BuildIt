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
using Microsoft.WindowsAzure.Storage;

namespace BuildIt.Backup.Azure.SqlDb
{
    public class BackupSqlDb
    {
        public static async Task InitiateDbBackup(
            string dbServerName,
            string dbName,
            string dbUsername,
            string dbPassword,
            AzureDbLocation dbLocation,
            string targetStorageAccountConnectionString,
            string targetContainerName,
            IDbBackupNotifier notifier,
            TextWriter log = null)
        {
            CloudStorageAccount targetStorageAccount;
            try
            {
                targetStorageAccount = CloudStorageAccount.Parse(targetStorageAccountConnectionString);
            }
            catch (Exception e)
            {
                var errorMessage =
                    "Unable to connect to target storage container to initiate backup. Check passed connection string values to ensure they are valid. \n" +
                    $"Exception thrown was of type {e.GetType().Name} with message: {e.Message} \n" +
                    $"Stacktrace for exception was {e.StackTrace}";
                await notifier.NotifyBackupError(dbServerName, dbName, Guid.Empty, errorMessage);
                log?.WriteLine(errorMessage);
                return;
            }

            var targetBlobClient = targetStorageAccount.CreateCloudBlobClient();
            var targetContainer = targetBlobClient.GetContainerReference(targetContainerName);
            log?.WriteLine($"Creating backup container with name: {targetContainerName}");
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

            var blobName = $"{dbName}/{DateTime.UtcNow:yyyy-MM-dd}/{DateTime.UtcNow:HH-mm-ss}/backup.bacpac";
            var credentials = new BlobStorageAccessKeyCredentials
            {
                Uri = $"{targetStorageAccount.BlobEndpoint}{targetContainerName}/{blobName}",
                StorageAccessKey = targetStorageAccount.Credentials.ExportBase64EncodedKey()
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
                log?.WriteLine($"Initiating backup for SQL Database: {dbName}");
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
                        log?.WriteLine($"Backup operation has been initiated with operation id: {operationId}");
                        await notifier.NotifyBackupInitiated(dbServerName, dbName, operationId);
                    }
                }
            }
            catch (Exception e)
            {
                var errorMessage =
                    $"Exception of type {e.GetType().Name} ocurred while backing up database: {dbName} \n" +
                    $"Exception message: {e.Message} \n" +
                    $"Exception stacktrace: {e.StackTrace}";
                log?.WriteLine(errorMessage);
                await notifier.NotifyBackupError(dbServerName, dbName, Guid.Empty, errorMessage);
            }
        }

        public static async Task<bool> MonitorBackupSinglePass(
            string dbServerName,
            string dbName,
            string dbUsername,
            string dbPassword,
            AzureDbLocation dbLocation,
            Guid operationId,
            IDbBackupNotifier notifier,
            TextWriter log = null)
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
                log?.WriteLine($"Checking status of backup operation {operationId} for database: {dbName}");
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
                                log?.WriteLine(errorMessage);
                                await notifier.NotifyBackupError(dbServerName, dbName, operationId, errorMessage);
                            }
                            else if (statusInfo.Status.Contains("Running"))
                            {
                                log?.WriteLine(
                                    $"Backup operation for databse: {dbName} is reporting status of: {statusInfo.Status}");
                                await notifier.NotifyBackupProgress(dbServerName, dbName, operationId, true);
                                pendingCopy = true;
                            }
                            else if (statusInfo.Status == "Completed")
                            {
                                log?.WriteLine($"Backup operation for database: {dbName} is complete.");
                                await notifier.NotifyBackupProgress(dbServerName, dbName, operationId, false);
                            }
                        }
                        else
                        {
                            log?.WriteLine($"Unable to obtain backup status information for database: {dbName} with operation id: {operationId}");
                            await notifier.NotifyBackupProgress(dbServerName, dbName, operationId, true);
                            pendingCopy = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var errorMessage =
                    $"Exception of type {e.GetType().Name} ocurred while backing up database: {dbName} \n" +
                    $"Exception message: {e.Message} \n" +
                    $"Exception stacktrace: {e.StackTrace}";
                log?.WriteLine(errorMessage);
                await notifier.NotifyBackupError(dbServerName, dbName, operationId, errorMessage);
            }

            return pendingCopy;
        }
    }
}
