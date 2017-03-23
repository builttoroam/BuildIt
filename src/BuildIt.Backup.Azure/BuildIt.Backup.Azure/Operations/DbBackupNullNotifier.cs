using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildIt.Backup.Azure.Operations
{
    /// <summary>
    /// A null implementation of the IDbBackupNotifier interface. This class does nothing.
    /// </summary>
    public class DbBackupNullNotifier : IDbBackupNotifier
    {
        public Task Init()
        {
            return Task.Delay(0);
        }

        public Task NotifyBackupInitiated(string dbServer, string dbName, string backupBlobName, Guid operationId)
        {
            return Task.Delay(0);
        }

        public Task NotifyBackupProgress(string dbServer, string dbName, string backupBlobName, Guid operationId, bool backupInProgress)
        {
            return Task.Delay(0);
        }

        public Task NotifyBackupError(string dbServer, string dbName, string backupBlobName, Guid operationId, string errorMessage)
        {
            return Task.Delay(0);
        }
    }
}
