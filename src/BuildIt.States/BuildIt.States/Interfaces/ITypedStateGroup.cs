using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// State group where the type of the state information is defined
    /// </summary>
    /// <typeparam name="TState">The type that's used to define the state</typeparam>
    /// <typeparam name="TStateDefinition">The type of the state definition</typeparam>
    /// <typeparam name="TStateGroupDefinition">The type of the group definition</typeparam>
    public interface ITypedStateGroup<TState, TStateDefinition, TStateGroupDefinition>
    : IStateGroup<TStateDefinition, TStateGroupDefinition>, INotifyTypedStateChange<TState>
        where TStateDefinition : class, IStateDefinition, new()
        where TStateGroupDefinition : class, IStateGroupDefinition<TStateDefinition>, new()
    {
        /// <summary>
        /// Gets the current state name
        /// </summary>
        TState CurrentState { get; }

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
        /// Change to typed state
        /// </summary>
        /// <param name="newState">The new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <param name="cancel">Cancellation token allowing change to be cancelled</param>
        /// <returns>Success if change is completed</returns>
        Task<bool> ChangeToState(TState newState, bool useTransitions, CancellationToken cancel);

        /// <summary>
        /// Change to typed state, with data
        /// </summary>
        /// <typeparam name="TData">The type of data to pass to new state</typeparam>
        /// <param name="newState">The new state</param>
        /// <param name="data">The data to pass to the new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <param name="cancel">Cancellation token allowing change to be cancelled</param>
        /// <returns>Success if change is completed</returns>
        Task<bool> ChangeToStateWithData<TData>(TState newState, TData data, bool useTransitions, CancellationToken cancel);

        /// <summary>
        /// Changes back to a typed state
        /// </summary>
        /// <param name="newState">The state to change back to</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <param name="cancel">Cancellation token allowing change to be cancelled</param>
        /// <returns>Success if change is completed</returns>
        Task<bool> ChangeBackToState(TState newState, bool useTransitions, CancellationToken cancel);
    }
}