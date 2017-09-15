using System;
using System.Linq;
using System.Threading.Tasks;

namespace BuildIt.Logging.Filters
{
    /// <summary>
    /// Filter that requires categories
    /// </summary>
    public class RequiredCategoriesLogFilter : BaseLogFilter
    {
        /// <summary>
        /// Gets or sets the required categories
        /// </summary>
        public string[] RequiredCategories { get; set; }

        /// <summary>
        /// Generates the filter function
        /// </summary>
        /// <returns>Filter function</returns>
        protected override Func<ILogEntry, Task<bool>> BuildFilter()
        {
            if (RequiredCategories?.Length == 0)
            {
                return base.BuildFilter();
            }

            bool Fms(ILogEntry entry) => RequiredCategories.All(cat => entry.Categories.Contains<string>(cat));

            return entry => Task.FromResult(Fms(entry));
        }
    }
}