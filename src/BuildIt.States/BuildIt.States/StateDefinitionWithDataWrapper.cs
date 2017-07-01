using System.ComponentModel;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    public class StateDefinitionWithDataWrapper<TData> : IStateDefinitionWithData<TData>
        where TData : INotifyPropertyChanged
    {
        public IStateDefinition State { get; set; }

        public IStateDefinitionTypedDataWrapper<TData> StateData
            => State.UntypedStateDataWrapper as IStateDefinitionTypedDataWrapper<TData>;
    }
}