using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace BackupWebjobSample
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            var host = new JobHost();

            // For the purposes of this sample, we are manually calling this function on app startup
            var callTask = host.CallAsync(typeof(Functions).GetMethod(nameof(Functions.InitiateBlobStorageBackup)));
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }
    }
}
