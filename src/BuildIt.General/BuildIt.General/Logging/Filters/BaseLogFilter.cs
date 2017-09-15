using System;
using System.Threading.Tasks;

namespace BuildIt.Logging.Filters
{
    /// <summary>
    /// Base log filter
    /// </summary>
    public class BaseLogFilter : ILogFilter
    {
        private Func<ILogEntry, Task<bool>> filter;

        /// <summary>
        /// Gets the filter function to determine if a log item should be logged
        /// </summary>
        protected Func<ILogEntry, Task<bool>> Filter => filter ?? (filter = BuildFilter());

        /// <summary>
        /// Method to determine if a log entry should be logged
        /// </summary>
        /// <param name="entry">The log entry</param>
        /// <returns>Task to await - success to include log entry</returns>
        public async Task<bool> IncludeLog(ILogEntry entry)
        {
            var f = Filter(entry);
            return f != null ? await f : await Task.FromResult(true);
        }

        /// <summary>
        /// Generates the filter function
        /// </summary>
        /// <returns>Filter function</returns>
        protected virtual Func<ILogEntry, Task<bool>> BuildFilter()
        {
            return entry => Task.FromResult(true);
        }
    }
}