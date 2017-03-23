using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildIt.Backup.Azure.Operations
{
    /// <summary>
    /// Interface for a notifier object that informs of database backup operations
    /// </summary>
    public interface IDbBackupNotifier
    {
        /// <summary>
        /// Used to initialize any required resources for the implemented notifier
        /// </summary>
        Task Init();

        /// <summary>
        /// Sends a notification via the implemented channel that an async copy command has been triggered for a database
        /// </summary>
        /// <param name="dbServer">Public server endpoint, eg. [ServerName].database.windows.net</param>
        /// <param name="dbName">Name of the database that is being backed up</param>
        /// <param name="backupBlobName">Full name of the generated .bacpac file</param>
        /// <param name="operationId">Identifier of the async backup operation on the DAC Service</param>
        Task NotifyBackupInitiated(
            string dbServer,
            string dbName,
            string backupBlobName,
            Guid operationId);

        /// <summary>
        /// Sends a notification via the implemented channel that an async copy command has completed, or is still in progress
        /// </summary>
        /// <param name="dbServer">Public server endpoint, eg. [ServerName].database.windows.net</param>
        /// <param name="dbName">Name of the database that is being backed up</param>
        /// <param name="backupBlobName">Full name of the generated .bacpac file</param>
        /// <param name="operationId">Identifier of the async backup operation on the DAC Service</param>
        /// <param name="backupInProgress">Whether the operation is complete, or still in progress</param>
        Task NotifyBackupProgress(
            string dbServer,
            string dbName,
            string backupBlobName,
            Guid operationId,
            bool backupInProgress);

        /// <summary>
        /// Sends a notification via the implemented channel that an error has ocurred during an async copy command.
        /// </summary>
        /// <param name="dbServer">Public server endpoint, eg. [ServerName].database.windows.net</param>
        /// <param name="dbName">Name of the database that is being backed up</param>
        /// <param name="backupBlobName">Full name of the generated .bacpac file</param>
        /// <param name="operationId">Identifier of the async backup operation on the DAC Service</param>
        /// <param name="errorMessage">The details of the error that has ocurred</param>
        Task NotifyBackupError(
            string dbServer,
            string dbName,
            string backupBlobName,
            Guid operationId,
            string errorMessage);
    }
}
