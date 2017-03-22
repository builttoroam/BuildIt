using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildIt.Backup.Azure
{
    internal static class Constants
    {
        internal static class DateFormats
        {
            internal const string BlobBackupContainerName = "yyyyMMdd-HHmmss";
        }

        internal static class ErrorMessages
        {
            internal const string CannotConnectMultipleContainers =
                "Unable to connect to the storage account for one of the passed connection strings. Check passed connection string values to ensure they are valid.";
            internal const string CannotConnectStorageAccount =
                "Unable to connect to target storage account. Check passed connection string values to ensure they are valid.";
        }

        internal static class Metadata
        {
            internal const string ContainerCreatedTimeKey = "BackupContainerCreated";
        }
    }
}
