using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// State group where the type of the state information is defined
    /// </summary>
    /// <typeparam name="TState">The type that's used to define the state</typeparam>
    public interface ITypedStateGroup<TState> : IStateGroup
    {
        /// <summary>
        /// Typed state changed event
        /// </summary>
        event EventHandler<TypedStateEventArgs<TState>> TypedStateChanged;

        /// <summary>
        /// Typed state changing event
        /// </summary>
        event EventHandler<TypedStateCancelEventArgs<TState>> TypedStateChanging;

        /// <summary>
        /// Gets the current state name
        /// </summary>
        TState CurrentState { get; }

        /// <summary>
        /// Gets the current state name
        /// </summary>
        ITypedStateDefinition<TState> CurrentTypedStateDefinition { get; }

        /// <summary>
        /// Gets returns the state definition for the current state
        /// </summary>
        IReadOnlyDictionary<TState, ITypedStateDefinition<TState>> TypedStates { get; }

        /// <summary>
        /// Defines all states for the enum type
        /// </summary>
        void DefineAllStates();

        /// <summary>
        /// Change to typed state
        /// </summary>
        /// <param name="newState">The new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Success if change is completed</returns>
        Task<bool> ChangeToState(TState newState, bool useTransitions = true);

        /// <summary>
        /// Change to typed state, with data
        /// </summary>
        /// <typeparam name="TData">The type of data to pass to new state</typeparam>
        /// <param name="newState">The new state</param>
        /// <param name="data">The data to pass to the new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Success if change is completed</returns>
        Task<bool> ChangeToStateWithData<TData>(TState newState, TData data, bool useTransitions = true);

        /// <summary>
        /// Changes back to a typed state
        /// </summary>
        /// <param name="newState">The state to change back to</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Success if change is completed</returns>
        Task<bool> ChangeBackToState(TState newState, bool useTransitions = true);

        /// <summary>
        /// Defines a state in the group
        /// </summary>
        /// <param name="stateDefinition">The state definition to define</param>
        /// <returns>The defined state definition (maybe existing if one has been previously defined)</returns>
        ITypedStateDefinition<TState> DefineTypedState(ITypedStateDefinition<TState> stateDefinition);

        /// <summary>
        /// Defines a state based on a name
        /// </summary>
        /// <param name="state">The name of the state</param>
        /// <returns>The defined state definition (maybe existing if one has been previously defined)</returns>
        ITypedStateDefinition<TState> DefineTypedState(TState state);

        /// <summary>
        /// Defines a state with data
        /// </summary>
        /// <typeparam name="TStateData">The type of state data</typeparam>
        /// <param name="state">The name of the state to define</param>
        /// <returns>The defined state definition (maybe existing if one has been previously defined)</returns>
        ITypedStateDefinitionWithData<TState, TStateData> DefineTypedStateWithData<TStateData>(TState state)
            where TStateData : INotifyPropertyChanged;
    }
}