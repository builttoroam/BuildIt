using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Defines a set of states
    /// </summary>
    /// <typeparam name="TStateDefinition">Type of the state definition</typeparam>
    /// <typeparam name="TStateGroupDefinition">Type of the group definition</typeparam>
    public interface IStateGroup<
        // ReSharper disable once TypeParameterCanBeVariant - Not required
        TStateDefinition,
        // ReSharper disable once TypeParameterCanBeVariant - Not required
        TStateGroupDefinition> : IStateGroup
        where TStateDefinition : class, IStateDefinition, new()
        where TStateGroupDefinition : class, IStateGroupDefinition<TStateDefinition>, new()
    {
        /// <summary>
        /// Gets the state group definition (including the states that make up the group)
        /// </summary>
        TStateGroupDefinition TypedGroupDefinition { get; }

        /// <summary>
        /// Gets the current state name
        /// </summary>
        TStateDefinition CurrentTypedStateDefinition { get; }
    }

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
        /// Gets the name of the state group
        /// </summary>
        string GroupName { get; }

        /// <summary>
        /// Gets the state group definition (including the states that make up the group)
        /// </summary>
        IStateGroupDefinition GroupDefinition { get; }

        /// <summary>
        /// Gets the current state data (ie associated with the current state)
        /// </summary>
        INotifyPropertyChanged CurrentStateData { get; }

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

        /// <summary>
        /// Gets the current state name
        /// </summary>
        string CurrentStateName { get; }

        /// <summary>
        /// Gets the current state name
        /// </summary>
        IStateDefinition CurrentStateDefinition { get; }

        /// <summary>
        /// Gets the targets to be used when changing state
        /// </summary>
        IDictionary<string, object> StateValueTargets { get; }

        /// <summary>
        /// Instigates a change to a state
        /// </summary>
        /// <param name="newState">The state to change to</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Successful change to state</returns>
        Task<bool> ChangeToStateByName(string newState, bool useTransitions = true);

        /// <summary>
        /// Instigates a change to a state
        /// </summary>
        /// <param name="newState">The state to change to</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <param name="cancel">Cancellation token allowing change to be cancelled</param>
        /// <returns>Successful change to state</returns>
        Task<bool> ChangeToStateByName(string newState, bool useTransitions, CancellationToken cancel);

        /// <summary>
        /// Instigates a change to state, passing data into new state
        /// </summary>
        /// <typeparam name="TData">The type of data to pass into the new state</typeparam>
        /// <param name="newState">The new state</param>
        /// <param name="data">The data to pass into the new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Successful change to state</returns>
        Task<bool> ChangeToStateByNameWithData<TData>(string newState, TData data, bool useTransitions = true);

        /// <summary>
        /// Instigates a change to state, passing data into new state
        /// </summary>
        /// <typeparam name="TData">The type of data to pass into the new state</typeparam>
        /// <param name="newState">The new state</param>
        /// <param name="data">The data to pass into the new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <param name="cancel">Cancellation token allowing change to be cancelled</param>
        /// <returns>Successful change to state</returns>
        Task<bool> ChangeToStateByNameWithData<TData>(string newState, TData data, bool useTransitions, CancellationToken cancel);

        /// <summary>
        /// Change to state by going back in history
        /// </summary>
        /// <param name="newState">The new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Successful change to state</returns>
        Task<bool> ChangeBackToStateByName(string newState, bool useTransitions = true);

        /// <summary>
        /// Change to state by going back in history
        /// </summary>
        /// <param name="newState">The new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <param name="cancel">Cancellation token allowing change to be cancelled</param>
        /// <returns>Successful change to state</returns>
        Task<bool> ChangeBackToStateByName(string newState, bool useTransitions, CancellationToken cancel);

        /// <summary>
        /// Change to the previous state - will fail if go to previous is being blocked
        /// </summary>
        /// <param name="useTransitions">Whether to use transitions </param>
        /// <returns>Successful change to previous state</returns>
        Task<bool> ChangeToPrevious(bool useTransitions = true);

        /// <summary>
        /// Change to the previous state - will fail if go to previous is being blocked
        /// </summary>
        /// <param name="useTransitions">Whether to use transitions </param>
        /// <param name="cancel">Cancellation token allowing change to be cancelled</param>
        /// <returns>Successful change to previous state</returns>
        Task<bool> ChangeToPrevious(bool useTransitions, CancellationToken cancel);

        /// <summary>
        /// Binds state groups
        /// </summary>
        /// <param name="groupToBindTo">The group to bind to</param>
        /// <param name="bothDirections">Whether groups are kept in sync in both directions</param>
        /// <returns>Binder object that can be used to unbind the groups</returns>
        Task<IStateBinder> Bind(IStateGroup groupToBindTo, bool bothDirections = true);

        /// <summary>
        ///     Adds a trigger to start monitoring
        /// </summary>
        /// <param name="trigger">The trigger to monitor</param>
        void WatchTrigger(IStateTrigger trigger);
    }
}