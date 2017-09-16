using System;
using System.Threading.Tasks;

namespace BuildIt.Logging
{
    /// <summary>
    /// Abstract implementation of the ILogService - useful when debugging the
    /// General library during development. Methods need to be overridden
    /// if using this class in a project
    /// </summary>
    public class BasicLoggerService : ILoggerService
    {
        /// <summary>
        /// Gets or sets the filter that's being applied to the logging
        /// </summary>
        public ILogFilter Filter { get; set; }

        /// <summary>
        /// Gets or sets a delegate that can be used to handle logging output
        /// </summary>
        public Func<ILogEntry, Task> LogOutput { get; set; }

        /// <summary>
        /// Handles logging the item (called by the logging infrastructure)
        /// </summary>
        /// <param name="logItem">The item to be logged</param>
        /// <returns>Task to await</returns>
        public async Task Log(ILogEntry logItem)
        {
            // NB: This is a null operation in the release method - this stub is only
            // here to facilitate debugging the General library
            System.Diagnostics.Debug.WriteLine(logItem?.ToString());

            if (LogOutput == null)
            {
                return;
            }

            await LogOutput(logItem);
        }
    }
}