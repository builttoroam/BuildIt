using System;
using System.ComponentModel;
using BuildIt.States.Interfaces.Builder;

namespace BuildIt.States
{
    public interface IStateWithDataAction<TState,TStateData>: 
        IStateDefinitionWithDataBuilder<TState, TStateData>
        where TStateData : INotifyPropertyChanged
        where TState : struct
    {
        Action<TStateData> WhenChangedToNewState(TState newState);
        Action<TStateData> WhenChangingFromNewState(TState newState);

        Action<TStateData> WhenChangedToPreviousState();
        Action<TStateData> WhenChangingFromPreviousState();
    }
}