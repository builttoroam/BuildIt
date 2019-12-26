using System.Threading.Tasks;

namespace BuildIt.Logging
{
    /// <summary>
    /// Log filter.
    /// </summary>
    public interface ILogFilter
    {
        /// <summary>
        /// Determines if an entry should be logged.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>Task to await - success to include entry.</returns>
        Task<bool> IncludeLog(ILogEntry entry);
    }
}