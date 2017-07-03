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
        /// The available state groups
        /// </summary>
        IReadOnlyDictionary<string, IStateGroup> StateGroups { get; }

        /// <summary>
        /// The enum state group for a type (enum)
        /// </summary>
        /// <typeparam name="TState">The type (enum) of the state group</typeparam>
        /// <returns>The state group</returns>
        IEnumStateGroup<TState> EnumStateGroup<TState>() where TState : struct;

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
        TState CurrentState<TState>() where TState : struct;

        /// <summary>
        /// The current state for a specific state group
        /// </summary>
        /// <param name="groupName">The name of the state group</param>
        /// <returns>The name of the current state</returns>
        string CurrentState(string groupName);

        Task<bool> GoToState(string groupName,string stateName, bool animate = true);
        Task<bool> GoToStateWithData<TData>(string groupName, string stateName, TData data, bool animate = true);
        Task<bool> GoBackToState(string groupName, string stateName, bool animate = true);


        Task<bool> GoToState<TState>(TState state, bool animate = true) where TState : struct;
        Task<bool> GoToStateWithData<TState,TData>(TState state, TData data, bool animate = true) where TState : struct;
        Task<bool> GoBackToState<TState>(TState state, bool animate = true) where TState : struct;

        Task<bool> GoBackToPreviousState(bool animate = true);

        bool PreviousStateExists { get; }

        bool GoToPreviousStateIsBlocked { get; }

        IStateBinder Bind(IStateManager managerToBindTo, bool bothDirections=true);
    }
}