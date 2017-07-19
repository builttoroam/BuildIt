using System;
using System.Collections.Generic;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    /// <summary>
    /// Definition of a state group
    /// </summary>
    public abstract class BaseStateGroupDefinition : IStateGroupDefinition
    {
        /// <summary>
        /// Gets the name of the state group
        /// </summary>
        public abstract string GroupName { get; }

        /// <summary>
        /// Gets dictionary of states that can be transitioned to
        /// </summary>
        public IDictionary<string, IStateDefinition> States { get; } =
            new Dictionary<string, IStateDefinition>();

        /// <summary>
        /// Gets internal dictionary of default property values - so that they can be
        /// unset in the case of transitioning to a state that doesn't define
        /// values for every property
        /// </summary>
        public IDictionary<Tuple<object, string>, IDefaultValue> DefaultValues { get; }
            = new Dictionary<Tuple<object, string>, IDefaultValue>();

        /// <summary>
        /// Retrieve state definition based on state name
        /// </summary>
        /// <param name="state">The state name</param>
        /// <returns>New state definition</returns>
        public IStateDefinition StateDefinition(string state)
        {
            return string.IsNullOrWhiteSpace(state) ? null : States.SafeValue(state);
        }
    }
}