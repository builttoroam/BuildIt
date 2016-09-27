using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Autofac;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public interface IGenerateViewModel
    {
        Type ViewModelType { get; }
        INotifyPropertyChanged Generate();

        Task InvokeInitialiseViewModel(INotifyPropertyChanged viewModel);

        Task InvokeAboutToChangeFromViewModel(INotifyPropertyChanged viewModel, CancelEventArgs cancel);

        Task InvokeChangingFromViewModel(INotifyPropertyChanged viewModel);

        Task InvokeChangedToViewModel(INotifyPropertyChanged viewModel);

        Task InvokeChangedToWithDataViewModel(INotifyPropertyChanged viewModel,string data);
    }
}