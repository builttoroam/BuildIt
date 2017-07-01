using System.ComponentModel;

namespace BuildIt.States.Interfaces
{
    public interface IStateDefinitionWithData<TData> where TData : INotifyPropertyChanged
    {
        IStateDefinition State { get; }

        IStateDefinitionTypedDataWrapper<TData> StateData { get; } 
    }
}