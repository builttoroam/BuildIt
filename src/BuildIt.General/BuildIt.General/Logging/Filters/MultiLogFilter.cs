using System;
using System.Threading.Tasks;

namespace BuildIt.Logging.Filters
{
    /// <summary>
    /// Filter that requires nested filters (abstract).
    /// </summary>
    public abstract class MultiLogFilter : BaseLogFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiLogFilter"/> class.
        /// </summary>
        /// <param name="filters">The nested filters.</param>
        protected MultiLogFilter(params ILogFilter[] filters)
        {
            Filters = filters;
        }

        /// <summary>
        /// Gets the nested filters.
        /// </summary>
        protected ILogFilter[] Filters { get; }

        /// <summary>
        /// Generates the filter function.
        /// </summary>
        /// <returns>Filter function.</returns>
        protected override Func<ILogEntry, Task<bool>> BuildFilter()
        {
            if (Filters?.Length == 0)
            {
                return base.BuildFilter();
            }

            return BuildMultiFilter();
        }

        /// <summary>
        /// The internal method to return the multi-filter function.
        /// </summary>
        /// <returns>The function used to combine multiple nested filters.</returns>
        protected abstract Func<ILogEntry, Task<bool>> BuildMultiFilter();
    }
}