using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using BuildIt.ServiceLocation;
using BuildIt.States.Interfaces;
using BuildIt.States.Interfaces.StateData;

namespace BuildIt.States
{
    /// <summary>
    /// Defines a group of states
    /// </summary>
    public class StateGroup : TypedStateGroup<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateGroup"/> class.
        /// Constructs a group based on the supplied group name
        /// </summary>
        /// <param name="groupName">The name of the state group</param>
        public StateGroup(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new ArgumentNullException(nameof(groupName), "Group name is required");
            }

            GroupName = groupName;
        }

        /// <summary>
        /// Gets the name of the state group
        /// </summary>
        public override string GroupName { get; }

        ///// <summary>
        ///// Add the state definition to the list of definitions. Returns the
        ///// existing state if one already exists with the same name
        ///// </summary>
        ///// <param name="stateDefinition">The state definition to register</param>
        ///// <returns>The registered state definition</returns>
        // public virtual IStateDefinition DefineState(IStateDefinition stateDefinition)
        // {
        //    if (stateDefinition == null)
        //    {
        //        "Can't define null state definition".Log();
        //        return null;
        //    }

        // var existing = StateDefinition(stateDefinition.StateName); // States.SafeValue(stateDefinition.StateName);
        //    if (existing != null)
        //    {
        //        $"State definition already defined, returning existing instance - {existing.GetType().Name}".Log();
        //        return existing;
        //    }

        // $"Defining state of type {stateDefinition.GetType().Name}".Log();
        //    States[stateDefinition.StateName] = stateDefinition;
        //    return stateDefinition;
        // }

        ///// <summary>
        ///// Create state definition
        ///// </summary>
        ///// <param name="state">The name of the state definition to create</param>
        ///// <returns>The new state definition</returns>
        // public ITypedStateDefinition<string> DefineState(string state)
        // {
        //    // Don't ever add the default value (eg Base) state
        //    if (string.IsNullOrWhiteSpace(state))
        //    {
        //        "Attempted to add state definition for null or empty state name".Log();
        //        return null;
        //    }

        // var stateDefinition = new TypedStateDefinition<string>(state);
        //    return DefineState(stateDefinition);
        // }

        ///// <summary>
        ///// Define a state with data entity
        ///// </summary>
        ///// <typeparam name="TStateData">The type of the data to be associated with the state</typeparam>
        ///// <param name="state">The name of the state</param>
        ///// <returns>Wrapper around the state definition and data wrapper</returns>
        // public virtual IStateDefinitionWithData<TStateData> DefineStateWithData<TStateData>(string state)
        //    where TStateData : INotifyPropertyChanged
        // {
        //    // Don't ever add the default value (eg Base) state
        //    if (string.IsNullOrWhiteSpace(state))
        //    {
        //        "Attempted to add state definition for null or empty state name".Log();
        //        return null;
        //    }

        // $"Defining state for {state}".Log();
        //    var stateDef = DefineState(new StateDefinition(state));
        //    if (stateDef is StateDefinition stateDefinition)
        //    {
        //        stateDefinition.UntypedStateDataWrapper = new StateDefinitionTypedDataWrapper<TStateData>();
        //    }

        // return new StateDefinitionWithDataWrapper<TStateData> { State = stateDef };
        // }

        /// <summary>
        /// Parses the state name and returns the state (in this case a string)
        /// </summary>
        /// <param name="stateName">The state name</param>
        /// <returns>The state</returns>
        protected override string FromString(string stateName)
        {
            return stateName;
        }
    }
}