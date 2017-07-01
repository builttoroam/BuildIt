using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces
{
    public interface IStateDefinitionDataWrapper
    {
        Type StateDataType { get; }
        INotifyPropertyChanged Generate();
        Task InvokeInitialise(INotifyPropertyChanged dataEntity);

        Task InvokeAboutToChangeFrom(INotifyPropertyChanged dataEntity, CancelEventArgs cancel);

        Task InvokeChangingFrom(INotifyPropertyChanged dataEntity);

        Task InvokeChangedTo(INotifyPropertyChanged dataEntity);

        Task InvokeChangedToWithData(INotifyPropertyChanged dataEntity, string data);
    }
}