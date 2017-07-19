using System;
using System.Collections.Generic;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Defines a state group (not an instance of a state group)
    /// </summary>
    public interface IStateGroupDefinition
    {
        /// <summary>
        /// Gets name of the group
        /// </summary>
        string GroupName { get; }

        /// <summary>
        /// Gets the states that have been defined
        /// </summary>
        IDictionary<string, IStateDefinition> States { get; }

        /// <summary>
        /// Gets the default values for the state group
        /// </summary>
        IDictionary<Tuple<object, string>, IDefaultValue> DefaultValues { get; }

        /// <summary>
        /// Retrieve state definition based on state name
        /// </summary>
        /// <param name="state">The state name</param>
        /// <returns>New state definition</returns>
        IStateDefinition StateDefinition(string state);
    }
}