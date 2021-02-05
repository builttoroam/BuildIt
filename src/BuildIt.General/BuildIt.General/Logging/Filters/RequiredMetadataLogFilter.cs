using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildIt.Logging.Filters
{
    /// <summary>
    /// Filter based on meta data.
    /// </summary>
    public class RequiredMetadataLogFilter : BaseLogFilter
    {
        /// <summary>
        /// Gets or sets the required metadata key/values - leave value empty if
        /// only requiring the key to be checked.
        /// </summary>
        public IDictionary<string, string> RequiredMetadata { get; set; }

        /// <summary>
        /// Generates the filter function.
        /// </summary>
        /// <returns>Filter function.</returns>
        protected override Func<ILogEntry, Task<bool>> BuildFilter()
        {
            if (RequiredMetadata?.Count == 0)
            {
                return base.BuildFilter();
            }

            bool Fms(ILogEntry entry) => RequiredMetadata.All(cat => entry.Metadata.ContainsKey(cat.Key) && (string.IsNullOrWhiteSpace(cat.Value) ||
                                                                                                             entry.Metadata[cat.Key] == cat.Value));

            return entry => Task.FromResult(Fms(entry));
        }
    }
}