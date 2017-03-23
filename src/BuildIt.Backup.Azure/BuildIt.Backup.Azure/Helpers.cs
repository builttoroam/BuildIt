using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BuildIt.Backup.Azure
{
    internal class Helpers
    {
        private const int CheckContainerExistsDelay = 5000;
        private const int MaximumContainerWaitLoops = 30;

        internal static bool ConnectStorageAccount(
            string sourceConnectionString,
            out CloudStorageAccount sourceStorageAccount,
            TraceWriter log)
        {
            var success = false;
            sourceStorageAccount = null;

            try
            {
                if (!string.IsNullOrEmpty(sourceConnectionString))
                {
                    sourceStorageAccount = CloudStorageAccount.Parse(sourceConnectionString);
                    if (sourceStorageAccount != null)
                    {
                        success = true;
                    }
                }
            }
            catch (Exception e)
            {
                log?.Error(e.ToString());
                success = false;
            }

            return success;
        }

        internal static async Task EnsureContainerExists(CloudBlobContainer targetContainer, TraceWriter log)
        {
            log?.Info($"Creating backup container with name: {targetContainer.Name}");
            await targetContainer.CreateIfNotExistsAsync();
            var containerExists = await targetContainer.ExistsAsync();
            var waitCount = 0;
            while (!containerExists)
            {
                await Task.Delay(CheckContainerExistsDelay);
                containerExists = await targetContainer.ExistsAsync();
                waitCount++;
                if (waitCount > MaximumContainerWaitLoops) // 2.5 minutes, really should have been created by now...
                {
                    throw new Exception("Unable to verify backup container");
                }
            }
        }
    }
}
