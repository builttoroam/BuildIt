using System.Collections.Generic;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Defines an entity that can be reset to its default value
    /// </summary>
    public interface IDefaultValue
    {
        /// <summary>
        /// Resets entity to default value
        /// </summary>
        /// <param name="targets">The set of target elements to use in state transition</param>
        void RevertToDefault(IDictionary<string, object> targets);
    }
}