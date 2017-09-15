using System;
using System.Threading.Tasks;

namespace BuildIt.Logging.Filters
{
    /// <summary>
    /// Filter that requires nested filters AND'd together
    /// </summary>
    public class AndLogFilter : MultiLogFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AndLogFilter"/> class.
        /// </summary>
        /// <param name="filters">The filters to AND</param>
        public AndLogFilter(params ILogFilter[] filters)
            : base(filters)
        {
        }

        /// <summary>
        /// Generates the filter function where there are multiple nested filters
        /// </summary>
        /// <returns>Filter function</returns>
        protected override Func<ILogEntry, Task<bool>> BuildMultiFilter()
        {
            async Task<bool> Fms(ILogEntry entry)
            {
                var include = true;
                await Filters.DoForEachAsync(async filter => include = include && await filter.IncludeLog(entry));
                return include;
            }

            return Fms;
        }
    }
}