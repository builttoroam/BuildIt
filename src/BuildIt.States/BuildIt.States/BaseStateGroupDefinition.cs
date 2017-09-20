using System;
using System.Collections.Generic;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    /// <summary>
    /// Definition of a state group
    /// </summary>
    /// <typeparam name="TStateDefinition">The type of the state definitions to create</typeparam>
    public abstract class BaseStateGroupDefinition<TStateDefinition>
        : IStateGroupDefinition<TStateDefinition>
        where TStateDefinition : class, IStateDefinition, new()
    {
        /// <summary>
        /// Gets or sets the name of the state group
        /// </summary>
        public virtual string GroupName { get; set; }

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
        /// <param name="stateName">The state name</param>
        /// <returns>New state definition</returns>
        public virtual IStateDefinition StateDefinitionFromName(string stateName)
        {
            return TypedStateDefinitionFromName(stateName);
        }

        /// <summary>
        /// Retrieve state definition based on state name
        /// </summary>
        /// <param name="stateName">The state name</param>
        /// <returns>New state definition</returns>
        public virtual TStateDefinition TypedStateDefinitionFromName(string stateName)
        {
            return string.IsNullOrWhiteSpace(stateName) ? default(TStateDefinition) : States.SafeValue(stateName) as TStateDefinition;
        }

        /// <summary>
        /// Defines a new typed state definition
        /// </summary>
        /// <param name="stateDefinition">The typed state definition to define</param>
        /// <returns>The defined state definition (maybe existing definition if already defined)</returns>
        public TStateDefinition DefineTypedState(TStateDefinition stateDefinition)
        {
            if (stateDefinition == null)
            {
                "Can't define null state definition".LogStateInfo();
                return null;
            }

            var existing = StateDefinitionFromName(stateDefinition.StateName);
            if (existing != null)
            {
                $"State definition already defined, returning existing instance - {existing.GetType().Name}".LogStateInfo();
                return existing as TStateDefinition;
            }

            $"Defining state of type {stateDefinition.GetType().Name}".LogStateInfo();
            States[stateDefinition.StateName] = stateDefinition;
            return stateDefinition;
        }

        /// <summary>
        /// Creates a new instance of a state definition
        /// </summary>
        /// <returns>The state definition</returns>
        protected virtual TStateDefinition CreateDefinition()
        {
            var stateDef = new TStateDefinition();
            return stateDef;
        }
    }
}