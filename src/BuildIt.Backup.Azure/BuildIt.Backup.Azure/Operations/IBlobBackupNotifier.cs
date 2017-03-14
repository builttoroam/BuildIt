using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildIt.Backup.Azure.Operations
{
    /// <summary>
    /// Interface for a notifier object that informs of blob backup operations.
    /// </summary>
    public interface IBlobBackupNotifier
    {
        /// <summary>
        /// Used to initialize any required resources for the implemented notifier
        /// </summary>
        Task Init();

        /// <summary>
        /// Sends a notification via the implemented channel that an async copy command has been triggered on every blob in the source container
        /// </summary>
        /// <param name="sourceStorageAccountName">Account name of the source blob container</param>
        /// <param name="targetStorageAccountName">Account name of the target blob container</param>
        /// <param name="sourceContainerName">Name of the source blob contianer</param>
        /// <param name="targetContainerName">Name of the source blob contianer</param>
        Task NotifyBackupInitiated(
            string sourceStorageAccountName,
            string targetStorageAccountName,
            string sourceContainerName,
            string targetContainerName);

        /// <summary>
        /// Sends a notification via the implemented channel that an async copy command has completed for all blobs, or is still in progress
        /// </summary>
        /// <param name="sourceStorageAccountName">Account name of the source blob container</param>
        /// <param name="targetStorageAccountName">Account name of the target blob container</param>
        /// <param name="sourceContainerName">Name of the source blob contianer</param>
        /// <param name="targetContainerName">Name of the source blob contianer</param>
        /// <param name="backupInProgress">Whether the operation is complete, or still in progress</param>
        Task NotifyBackupProgress(
            string sourceStorageAccountName,
            string targetStorageAccountName,
            string sourceContainerName,
            string targetContainerName,
            bool backupInProgress);

        /// <summary>
        /// Sends a notification via the implemented channel that an error has ocurred during an async copy command.
        /// </summary>
        /// <param name="sourceStorageAccountName">Account name of the source blob container</param>
        /// <param name="targetStorageAccountName">Account name of the target blob container</param>
        /// <param name="sourceContainerName">Name of the source blob contianer</param>
        /// <param name="targetContainerName">Name of the source blob contianer</param>
        /// <param name="errorMessage">The details of the error that has ocurred</param>
        Task NotifyBackupError(
            string sourceStorageAccountName,
            string targetStorageAccountName,
            string sourceContainerName,
            string targetContainerName,
            string errorMessage);
    }
}
