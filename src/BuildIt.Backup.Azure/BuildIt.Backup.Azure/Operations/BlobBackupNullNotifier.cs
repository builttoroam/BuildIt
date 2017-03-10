using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildIt.Backup.Azure.Operations
{
    /// <summary>
    /// A null implementation of the IBlobBackupNotifier interface. This class does nothing.
    /// </summary>
    public class BlobBackupNullNotifier : IBlobBackupNotifier
    {
        public Task Init()
        {
            return Task.Delay(0);
        }

        public Task NotifyBackupInitiated(string sourceStorageAccountName, string targetStorageAccountName, string sourceContainerName,
            string targetContainerName)
        {
            return Task.Delay(0);
        }

        public Task NotifyBackupProgress(string sourceStorageAccountName, string targetStorageAccountName, string sourceContainerName,
            string targetContainerName, bool backupCompleted)
        {
            return Task.Delay(0);
        }
    }
}
