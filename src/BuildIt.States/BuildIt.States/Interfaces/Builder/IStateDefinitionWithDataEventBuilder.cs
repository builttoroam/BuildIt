using System;
using System.ComponentModel;

namespace BuildIt.States.Interfaces.Builder
{
    public interface IStateDefinitionWithDataEventBuilder<TState, TStateData> : 
        IStateWithDataAction<TState,TStateData>
        where TStateData : INotifyPropertyChanged
        where TState : struct
    {
        Action<TStateData, EventHandler> Subscribe { get; }
        Action<TStateData, EventHandler> Unsubscribe { get; }

    }
}