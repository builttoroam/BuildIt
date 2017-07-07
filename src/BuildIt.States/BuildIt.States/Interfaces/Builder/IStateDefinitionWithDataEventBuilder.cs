using System;
using System.ComponentModel;

namespace BuildIt.States.Interfaces.Builder
{
    /// <summary>
    /// Builder for state definition with events
    /// </summary>
    /// <typeparam name="TState">The type (enum) of state</typeparam>
    /// <typeparam name="TStateData">The type of the state data</typeparam>
    public interface IStateDefinitionWithDataEventBuilder<TState, TStateData> : 
        IStateWithDataActionBuilder<TState,TStateData>
        where TStateData : INotifyPropertyChanged
        where TState : struct
    {
        /// <summary>
        /// Subscribe entity to an event
        /// </summary>
        Action<TStateData, EventHandler> Subscribe { get; }

        /// <summary>
        /// Unsubscribe entity from an event
        /// </summary>
        Action<TStateData, EventHandler> Unsubscribe { get; }

    }
}