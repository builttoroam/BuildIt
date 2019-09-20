using System;
using System.Threading.Tasks;

namespace BuildIt.Logging.Filters
{
    /// <summary>
    /// Filter that inverts another filter.
    /// </summary>
    public class NotLogFilter : BaseLogFilter
    {
        /// <summary>
        /// Gets or sets the filter to invert.
        /// </summary>
        public ILogFilter FilterToInvert { get; set; }

        /// <summary>
        /// Generates the filter function.
        /// </summary>
        /// <returns>Filter function.</returns>
        protected override Func<ILogEntry, Task<bool>> BuildFilter()
        {
            if (FilterToInvert == null)
            {
                return base.BuildFilter();
            }

            async Task<bool> Fms(ILogEntry entry) => !(await FilterToInvert.IncludeLog(entry));

            return Fms;
        }
    }
}