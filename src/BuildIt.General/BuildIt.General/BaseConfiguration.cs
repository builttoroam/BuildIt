using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace BuildIt
{
    /// <summary>
    /// Base configuration class used to define configuration values
    /// </summary>
    public abstract class BaseConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseConfiguration"/> class.
        /// Constructs the configuration based on a set of property initializers - used to generate the
        /// lookup key (name of the function) and corresponding values
        /// </summary>
        /// <param name="initializers">The collection of property initializers</param>
        protected BaseConfiguration(IDictionary<Expression<Func<string>>, string> initializers = null)
        {
            initializers?.DoForEach(initializer =>
                Data[(initializer.Key.Body as MemberExpression)?.Member.Name] = initializer.Value);
        }

        /// <summary>
        /// Gets the collection of raw configuration values
        /// </summary>
        protected IDictionary<string, string> Data { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Retrieves a configuration value by name
        /// </summary>
        /// <param name="propertyName">The configuration value to return</param>
        /// <returns>The current configuration value</returns>
        protected string Value([CallerMemberName] string propertyName = null)
        {
            return propertyName == null ? null : Data.SafeValue<string, string, string>(propertyName);
        }
    }
}
