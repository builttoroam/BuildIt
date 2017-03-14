using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildIt.Backup.Azure.Operations
{
    public enum BackupOperationType
    {
        Undefined,
        Initiated,
        InProgress,
        Complete,
        Error
    }
}
