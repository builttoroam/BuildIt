using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Autofac;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public interface IGenerateViewModel
    {
        Type ViewModelType { get; }
        Task<INotifyPropertyChanged> Generate(IContainer dependency);

        Task InvokeAboutToChangeFromViewModel(INotifyPropertyChanged viewModel, CancelEventArgs cancel);

        Task InvokeChangingFromViewModel(INotifyPropertyChanged viewModel);

        Task InvokeChangedToViewModel(INotifyPropertyChanged viewModel);

    }
}