using System.ComponentModel;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    public class EnumStateDefinitionWithDataWrapper<TState, TData> : StateDefinitionWithDataWrapper<TData> , IEnumStateDefinitionWithData<TState, TData>
        where TData : INotifyPropertyChanged
        where TState : struct
    {
        public IEnumStateDefinition<TState> EnumState { get; set; }

        public IStateDefinitionTypedDataWrapper<TData> StateDataWrapper
            => State.UntypedStateDataWrapper as IStateDefinitionTypedDataWrapper<TData>;
    }
}