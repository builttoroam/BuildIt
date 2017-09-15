using System;
using System.Threading.Tasks;

namespace BuildIt.Logging.Filters
{
    /// <summary>
    /// Filter that requires a specific assembly name
    /// </summary>
    public class AssemblyNameLogFilter : BaseLogFilter
    {
        /// <summary>
        /// Gets or sets the assembly name
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// Generates the filter function
        /// </summary>
        /// <returns>Filter function</returns>
        protected override Func<ILogEntry, Task<bool>> BuildFilter()
        {
            if (string.IsNullOrWhiteSpace(AssemblyName))
            {
                return base.BuildFilter();
            }

            bool Fms(ILogEntry entry) => entry.AssemblyName == AssemblyName;

            return entry => Task.FromResult(Fms(entry));
        }
    }
}