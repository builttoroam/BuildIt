using System.Threading.Tasks;

namespace BuildIt.Logging
{
    /// <summary>
    /// Interface for a service that can handle logging
    /// </summary>
    public interface ILoggerService
    {
        /// <summary>
        /// Gets or sets filter to determine if an entry should be logged
        /// </summary>
        ILogFilter Filter { get; set; }

        /// <summary>
        /// Logs an entry
        /// </summary>
        /// <param name="logItem">The entry to log</param>
        /// <returns>Task to await</returns>
        Task Log(ILogEntry logItem);
    }
}