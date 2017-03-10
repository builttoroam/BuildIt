using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildIt.Backup.Azure.Operations
{
    public interface IBlobBackupNotifier
    {
        Task Init();

        Task NotifyBackupInitiated(
            string sourceStorageAccountName,
            string targetStorageAccountName,
            string sourceContainerName,
            string targetContainerName);
    }
}
