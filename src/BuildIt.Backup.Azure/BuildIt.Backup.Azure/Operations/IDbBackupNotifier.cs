using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildIt.Backup.Azure.Operations
{
    public interface IDbBackupNotifier
    {
        Task Init();

        Task NotifyBackupInitiated(
            string dbServer,
            string dbName,
            Guid operationId);

        Task NotifyBackupProgress(
            string dbServer,
            string dbName,
            Guid operationId,
            bool backupInProgress);

        Task NotifyBackupError(
            string dbServer,
            string dbName,
            Guid operationId,
            string errorMessage);
    }
}
