using System;
using System.ComponentModel;
using System.Threading;

namespace BuildIt.States.Interfaces.Builder
{
    /// <summary>
    /// Builder for state with actions for the state data
    /// </summary>
    /// <typeparam name="TState">The type (enum) of the state</typeparam>
    /// <typeparam name="TStateData">The type of the state data</typeparam>
    public interface IStateWithDataActionBuilder<TState, TStateData> :
        IStateDefinitionWithDataBuilder<TState, TStateData>
        where TStateData : INotifyPropertyChanged
        where TState : struct
    {
        /// <summary>
        /// Returns method to call when changed to a new state
        /// </summary>
        /// <param name="newState">The new typed state</param>
        /// <returns>Method to call</returns>
        Action<TStateData, CancellationToken> WhenChangedToNewState(TState newState);

        /// <summary>
        /// Returns method to call when changing from a state
        /// </summary>
        /// <param name="newState">The typed state</param>
        /// <returns>Method to call</returns>
        Action<TStateData, CancellationToken> WhenChangingFromNewState(TState newState);

        /// <summary>
        /// Returns method to call when changed to previous state
        /// </summary>
        /// <returns>Method to call</returns>
        Action<TStateData, CancellationToken> WhenChangedToPreviousState();

        /// <summary>
        /// Returns method to call when changing from previous state
        /// </summary>
        /// <returns>Method to call</returns>
        Action<TStateData, CancellationToken> WhenChangingFromPreviousState();
    }
}