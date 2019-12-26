using System;
using System.Threading.Tasks;

namespace BuildIt.Logging.Filters
{
    /// <summary>
    /// Filter that requires nested filters based on an OR.
    /// </summary>
    public class OrLogFilter : MultiLogFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrLogFilter"/> class.
        /// </summary>
        /// <param name="filters">The filters to OR.</param>
        public OrLogFilter(params ILogFilter[] filters)
            : base(filters)
        {
        }

        /// <summary>
        /// Generates the filter function where there are multiple nested filters.
        /// </summary>
        /// <returns>Filter function.</returns>
        protected override Func<ILogEntry, Task<bool>> BuildMultiFilter()
        {
            async Task<bool> Fms(ILogEntry entry)
            {
                foreach (var filter in Filters)
                {
                    if (await filter.IncludeLog(entry))
                    {
                        return true;
                    }
                }

                return false;
            }

            return Fms;
        }
    }
}