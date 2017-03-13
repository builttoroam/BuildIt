using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BuildIt.Backup.Azure.Operations
{
    public enum BlobBackupOperationType
    {
        Undefined,
        Initiated,
        InProgress,
        Complete,
        Error
    }

    public class BlobBackupOperationNotification
    {
        public BlobBackupOperationNotification(
            BlobBackupOperationType operationType,
            string sourceStorageAccountName,
            string targetStorageAccountName,
            string sourceContainerName,
            string targetContainerName,
            string errorMessage)
        {
            OperationType = operationType;
            SourceStorageAccountName = sourceStorageAccountName;
            TargetStorageAccountName = targetStorageAccountName;
            SourceContainerName = sourceContainerName;
            TargetContainerName = targetContainerName;
            ErrorMessage = errorMessage;
        }

        [JsonConverter(typeof(StringEnumConverter))] // Serializing as string for message readability
        public BlobBackupOperationType OperationType { get; }
        public string SourceStorageAccountName { get; }
        public string TargetStorageAccountName { get; }
        public string SourceContainerName { get; }
        public string TargetContainerName { get; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ErrorMessage { get; }
    }
}
