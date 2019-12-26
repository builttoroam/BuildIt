using System;
using System.Threading.Tasks;

namespace BuildIt.Logging.Filters
{
    /// <summary>
    /// Filter that requires a minimum log level.
    /// </summary>
    public class MinimumLogLevelLogFilter : BaseLogFilter
    {
        /// <summary>
        /// Gets or sets the log level to require.
        /// </summary>
        public LogLevel MinimumLogLevel { get; set; }

        /// <summary>
        /// Generates the filter function.
        /// </summary>
        /// <returns>Filter function.</returns>
        protected override Func<ILogEntry, Task<bool>> BuildFilter()
        {
            if (MinimumLogLevel == LogLevel.None)
            {
                return base.BuildFilter();
            }

            bool Fms(ILogEntry entry) => entry.Level >= MinimumLogLevel;

            return entry => Task.FromResult(Fms(entry));
        }
    }
}