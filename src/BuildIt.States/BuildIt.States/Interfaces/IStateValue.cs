using System;
using System.Collections.Generic;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Value definition for a state
    /// </summary>
    public interface IStateValue
    {
        /// <summary>
        /// Gets uniqueness key for the value definition
        /// </summary>
        Tuple<object, string> Key { get; }

        /// <summary>
        /// Method to invoke the change of value
        /// </summary>
        /// <param name="defaultValues">Default values to track original values of properties so they can be reverted</param>
        void TransitionTo(IDictionary<Tuple<object, string>, IDefaultValue> defaultValues);
    }
}