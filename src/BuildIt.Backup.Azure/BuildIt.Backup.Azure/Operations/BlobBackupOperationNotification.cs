using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildIt.Backup.Azure.Operations
{
    public enum BlobBackupOperationType
    {
        Undefined,
        BackupInitiated,
        BackupInProgress,
        BackupComplete
    }

    public class BlobBackupOperationNotification
    {
        public BlobBackupOperationNotification(
            BlobBackupOperationType operationType,
            string sourceStorageAccountName,
            string targetStorageAccountName, 
            string sourceContainerName,
            string targetContainerName)
        {
            OperationType = operationType;
            SourceStorageAccountName = sourceStorageAccountName;
            TargetStorageAccountName = targetStorageAccountName;
            SourceContainerName = sourceContainerName;
            TargetContainerName = targetContainerName;
        }

        public BlobBackupOperationType OperationType { get; }
        public string SourceStorageAccountName { get; }
        public string TargetStorageAccountName { get; }
        public string SourceContainerName { get; }
        public string TargetContainerName { get; }
    }
}
