using System.ComponentModel;

namespace BuildIt.States.Interfaces
{
    public interface IEnumStateDefinitionWithData<TState, TData>:IStateDefinitionWithData<TData>
        where TData : INotifyPropertyChanged
        where TState : struct
    {
        IEnumStateDefinition<TState> EnumState { get; }
    }
}