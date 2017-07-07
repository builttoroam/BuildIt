using System.ComponentModel;

namespace BuildIt.States.Interfaces.Builder
{
    public interface IStateDefinitionWithDataChangeStateWithDataBuilder<TState, TStateData, TNewStateData> :
        IStateDefinitionWithDataBuilder<TState, TStateData>
        where TStateData : INotifyPropertyChanged
        where TState : struct
    {
        TState NewState { get; }
    }
}