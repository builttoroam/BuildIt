using System;
using System.Collections.Generic;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Value definition for a state.
    /// </summary>
    public interface IStateValue
    {
        /// <summary>
        /// Gets uniqueness key for the value definition.
        /// </summary>
        Tuple<object, string> Key { get; }

        /// <summary>
        /// Performs the state transition.
        /// </summary>
        /// <param name="targets">The set of target elements to use in state transition.</param>
        /// <param name="defaultValues">The set of default values to apply if state doesn't define property value.</param>
        void TransitionTo(IDictionary<string, object> targets, IDictionary<Tuple<object, string>, IDefaultValue> defaultValues);
    }
}