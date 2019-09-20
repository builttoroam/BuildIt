using System;
using System.Threading.Tasks;

namespace BuildIt.Logging.Filters
{
    /// <summary>
    /// Filter that requires a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to require.</typeparam>
    public class EntityTypeLogFilter<TEntity> : BaseLogFilter
    {
        /// <summary>
        /// Generates the filter function.
        /// </summary>
        /// <returns>Filter function.</returns>
        protected override Func<ILogEntry, Task<bool>> BuildFilter()
        {
            bool Fms(ILogEntry entry) => entry is ITypedLogEntry<TEntity> tentity && tentity.Entity != null;

            return entry => Task.FromResult(Fms(entry));
        }
    }
}