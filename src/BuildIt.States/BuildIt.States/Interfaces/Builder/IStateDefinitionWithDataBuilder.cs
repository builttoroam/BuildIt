using System.ComponentModel;

namespace BuildIt.States.Interfaces.Builder
{
    public interface IStateDefinitionWithDataBuilder<TState, TData> : IStateDefinitionBuilder<TState>
        where TData : INotifyPropertyChanged
        where TState : struct
    {
        IStateDefinitionTypedDataWrapper<TData> StateDataWrapper { get; }
    }
}