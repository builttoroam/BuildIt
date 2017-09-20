using System;

namespace BuildIt.States.Typed.String
{
    /// <summary>
    /// Definition of a string state group
    /// </summary>
    public class StateGroupDefinition
        : BaseStateGroupDefinition<StateDefinition>
    {
        /// <summary>
        /// Creates a new state definition based on the name of the state
        /// </summary>
        /// <param name="stateName">The name of the state to create</param>
        /// <returns>The created state</returns>
        public virtual StateDefinition CreateDefinitionFromName(string stateName)
        {
            var stateDef = CreateDefinition();
            stateDef.StateName = stateName;
            return stateDef;
        }

        /// <summary>
        /// Define a new typed state defintion based on an enum value
        /// </summary>
        /// <param name="stateName">The name of the state</param>
        /// <returns>The defined state definition (maybe existing definition if already defined)</returns>
        public StateDefinition DefineStateFromName(string stateName)
        {
            // Don't ever add the default value (eg Base) state
            if (string.IsNullOrWhiteSpace(stateName))
            {
                "Attempted to add the default state definition".LogStateInfo();
                return null;
            }

            var stateDefinition = CreateDefinitionFromName(stateName);
            $"Defined state for {stateName}".LogStateInfo();
            return DefineTypedState(stateDefinition);
        }
    }
}