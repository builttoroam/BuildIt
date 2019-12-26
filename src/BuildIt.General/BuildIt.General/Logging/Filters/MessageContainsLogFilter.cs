using System;
using System.Threading.Tasks;

namespace BuildIt.Logging.Filters
{
    /// <summary>
    /// Filter that requires a specific message contents.
    /// </summary>
    public class MessageContainsLogFilter : BaseLogFilter
    {
        /// <summary>
        /// Gets or sets a value indicating whether gets or sets whether the message content check is case sensitive (default is false).
        /// </summary>
        public bool MessageContainsCaseSensitive { get; set; }

        /// <summary>
        /// Gets or sets the message contents to check for.
        /// </summary>
        public string MessageContains { get; set; }

        /// <summary>
        /// Generates the filter function.
        /// </summary>
        /// <returns>Filter function.</returns>
        protected override Func<ILogEntry, Task<bool>> BuildFilter()
        {
            if (string.IsNullOrWhiteSpace(MessageContains))
            {
                return base.BuildFilter();
            }

            bool Fm(ILogEntry entry) => !string.IsNullOrWhiteSpace(entry.Message);
            Func<ILogEntry, bool> fms;
            if (MessageContainsCaseSensitive)
            {
                fms = entry => Fm(entry)
                               && entry.Message.IndexOf(MessageContains, StringComparison.Ordinal) >= 0;
            }
            else
            {
                fms = entry => Fm(entry)
                               && entry.Message.IndexOf(MessageContains, StringComparison.OrdinalIgnoreCase) >= 0;
            }

            return entry => Task.FromResult(fms(entry));
        }
    }
}