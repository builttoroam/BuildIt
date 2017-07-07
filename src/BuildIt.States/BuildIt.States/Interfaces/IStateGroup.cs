using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using BuildIt.ServiceLocation;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Defines a set of states
    /// </summary>
    public interface IStateGroup :
        IRequiresUIAccess,
        IRegisterDependencies,
        INotifyStateChanged,
        INotifyStateChanging
    {
        /// <summary>
        /// Event indicating the go to previous blocked status has changed
        /// </summary>
        event EventHandler GoToPreviousStateIsBlockedChanged;

        /// <summary>
        /// Gets name of the group
        /// </summary>
        string GroupName { get; }

        /// <summary>
        /// Gets the current state data (ie associated with the current state)
        /// </summary>
        INotifyPropertyChanged CurrentStateData { get; }

        /// <summary>
        /// Instigates a change to a state
        /// </summary>
        /// <param name="newState">The state to change to</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Successful change to state</returns>
        Task<bool> ChangeTo(string newState, bool useTransitions = true);

        /// <summary>
        /// Instigates a change to state, passing data into new state
        /// </summary>
        /// <typeparam name="TData">The type of data to pass into the new state</typeparam>
        /// <param name="newState">The new state</param>
        /// <param name="data">The data to pass into the new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Successful change to state</returns>
        Task<bool> ChangeToWithData<TData>(string newState, TData data, bool useTransitions = true);

        /// <summary>
        /// Change to state by going back in history
        /// </summary>
        /// <param name="newState">The new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Successful change to state</returns>
        Task<bool> ChangeBackTo(string newState, bool useTransitions = true);

        /// <summary>
        /// Change to the previous state - will fail if go to previous is being blocked
        /// </summary>
        /// <param name="useTransitions">Whether to use transitions </param>
        /// <returns>Successful change to previous state</returns>
        Task<bool> ChangeToPrevious(bool useTransitions = true);

        /// <summary>
        /// Binds state groups
        /// </summary>
        /// <param name="groupToBindTo">The group to bind to</param>
        /// <param name="bothDirections">Whether groups are kept in sync in both directions</param>
        /// <returns>Binder object that can be used to unbind the groups</returns>
        IStateBinder Bind(IStateGroup groupToBindTo, bool bothDirections = true);

        /// <summary>
        /// Gets or sets a value indicating whether gets/Sets whether state change history is tracked
        /// </summary>
        bool TrackHistory { get; set; }

        /// <summary>
        /// Gets a value indicating whether indicates whether there is a state change history
        /// </summary>
        bool HasHistory { get; }

        /// <summary>
        /// Gets a value indicating whether indicates whether go to previous is being blocked
        /// </summary>
        bool GoToPreviousStateIsBlocked { get; }

        //        IDependencyContainer DependencyContainer { get; set; }


        /// <summary>
        /// Defines a state in the group
        /// </summary>
        /// <param name="stateDefinition">The state definition to define</param>
        /// <returns>The defined state definition (maybe existing if one has been previously defined)</returns>
        IStateDefinition DefineState(IStateDefinition stateDefinition);

        /// <summary>
        /// Defines a state based on a name
        /// </summary>
        /// <param name="state">The name of the state</param>
        /// <returns>The defined state definition (maybe existing if one has been previously defined)</returns>
        IStateDefinition DefineState(string state);

        /// <summary>
        /// Defines a state with data
        /// </summary>
        /// <typeparam name="TStateData">The type of state data</typeparam>
        /// <param name="state">The name of the state to define</param>
        /// <returns>The defined state definition (maybe existing if one has been previously defined)</returns>
        IStateDefinitionWithData<TStateData> DefineStateWithData<TStateData>(string state)
            where TStateData : INotifyPropertyChanged;

        /// <summary>
        /// Gets the current state name
        /// </summary>
        string CurrentStateName { get; }

        /// <summary>
        /// Adds a trigger to start monitoring
        /// </summary>
        /// <param name="trigger">The trigger to monitor</param>
        void WatchTrigger(IStateTrigger trigger);

        /// <summary>
        /// Gets the states that have been defined
        /// </summary>
        IDictionary<string, IStateDefinition> States { get; }
    }
}