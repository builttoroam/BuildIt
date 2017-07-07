using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Defines the methods/properties of a state manager
    /// </summary>
    public interface IStateManager
    {
        /// <summary>
        /// Notifies that one of the current states is blocking the ability to go back to previous state
        /// </summary>
        event EventHandler GoToPreviousStateIsBlockedChanged;

        /// <summary>
        /// Gets the available state groups
        /// </summary>
        IReadOnlyDictionary<string, IStateGroup> StateGroups { get; }

        /// <summary>
        /// Gets a value indicating whether indicates if there is a previous state (in any state group)
        /// </summary>
        bool PreviousStateExists { get; }

        /// <summary>
        /// Gets a value indicating whether whether going to previous state is currently blocked
        /// </summary>
        bool GoToPreviousStateIsBlocked { get; }

        /// <summary>
        /// The enum state group for a type (enum)
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state group</typeparam>
        /// <returns>The state group</returns>
        IEnumStateGroup<TState> EnumStateGroup<TState>()
            where TState : struct;

        /// <summary>
        /// The state group by name
        /// </summary>
        /// <param name="groupName">The name of the state group</param>
        /// <returns>The state group</returns>
        IStateGroup StateGroup(string groupName);

        /// <summary>
        /// Adds a group based on type of state
        /// </summary>
        /// <param name="group">The group to add</param>
        void AddStateGroup(IStateGroup group);

        /// <summary>
        /// The current state for a specific type (ie group)
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the group</typeparam>
        /// <returns>The enum that represents the current state</returns>
        TState CurrentState<TState>()
            where TState : struct;

        /// <summary>
        /// The current state for a specific state group
        /// </summary>
        /// <param name="groupName">The name of the state group</param>
        /// <returns>The name of the current state</returns>
        string CurrentState(string groupName);

        /// <summary>
        /// Go to a new state
        /// </summary>
        /// <param name="groupName">The state group name</param>
        /// <param name="stateName">The state name</param>
        /// <param name="animate">Whether to animate transition</param>
        /// <returns>Whether the transition was successful</returns>
        Task<bool> GoToState(string groupName, string stateName, bool animate = true);

        /// <summary>
        /// Go to a new state, passing in data
        /// </summary>
        /// <typeparam name="TData">The type of data to be passed to the new state</typeparam>
        /// <param name="groupName">The state group name</param>
        /// <param name="stateName">The state name</param>
        /// <param name="data">The data to be passed in</param>
        /// <param name="animate">Whether to animate transition</param>
        /// <returns>Whether the transition was successful</returns>
        Task<bool> GoToStateWithData<TData>(string groupName, string stateName, TData data, bool animate = true);

        /// <summary>
        /// Go to state by going back over history of state changes
        /// </summary>
        /// <param name="groupName">The state group name</param>
        /// <param name="stateName">The state name</param>
        /// <param name="animate">Whether the transition should be animated</param>
        /// <returns>Whether the transition was successful</returns>
        Task<bool> GoBackToState(string groupName, string stateName, bool animate = true);

        /// <summary>
        /// Go to a new state
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state to go to</typeparam>
        /// <param name="state">The state to go to</param>
        /// <param name="animate">Whether to animate the transition</param>
        /// <returns>Whether the transition was successful</returns>
        Task<bool> GoToState<TState>(TState state, bool animate = true)
            where TState : struct;

        /// <summary>
        /// Transitions to a new state, passing in data
        /// </summary>
        /// <typeparam name="TState">The type (enum) of state to go to</typeparam>
        /// <typeparam name="TData">The type of data to be passed to new state</typeparam>
        /// <param name="state">The new state to go to</param>
        /// <param name="data">The data to pass to the new state</param>
        /// <param name="animate">Whether the transition should be animated</param>
        /// <returns>Whether the transition was successful</returns>
        Task<bool> GoToStateWithData<TState, TData>(TState state, TData data, bool animate = true)
            where TState : struct;

        /// <summary>
        /// Go to state by going back over history of state changes
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state to go to</typeparam>
        /// <param name="state">The state to go to</param>
        /// <param name="animate">Whether the transition should be animated</param>
        /// <returns>Whether the transition was successful</returns>
        Task<bool> GoBackToState<TState>(TState state, bool animate = true)
            where TState : struct;

        /// <summary>
        /// Go back to the previous state
        /// </summary>
        /// <param name="animate">whether to animate the transition</param>
        /// <returns>Whether the transition was successful</returns>
        Task<bool> GoBackToPreviousState(bool animate = true);

        /// <summary>
        /// Bind two different state managers
        /// </summary>
        /// <param name="managerToBindTo">The state manager to listen to for changes</param>
        /// <param name="bothDirections">Whether updates to states should go both ways</param>
        /// <returns>Binder that can be used to disconnect the state managers</returns>
        IStateBinder Bind(IStateManager managerToBindTo, bool bothDirections = true);
    }
}