using System;
using System.ComponentModel;
using System.Threading.Tasks;
using BuildIt.ServiceLocation;
using BuildIt.States;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public class ViewModelStateDefinition<TState, TViewModel> :
        EnumStateDefinition<TState>,
        IViewModelStateDefinition<TState, TViewModel>
        where TState : struct
        where TViewModel : INotifyPropertyChanged 
    {
        public Type ViewModelType => typeof(TViewModel);

        public INotifyPropertyChanged Generate()
        {
            $"Creating instance of {typeof(TViewModel).Name}".Log();
            var vm = ServiceLocator.Current.GetInstance<TViewModel>();

            
            "ViewModel generated".Log();
            return vm;
        }

        public Func<TViewModel, Task> InitialiseViewModel { get; set; }


        public async Task InvokeInitialiseViewModel(INotifyPropertyChanged viewModel)
        {
            if (InitialiseViewModel == null) return;

            "Invoking InitialiseViewModel".Log();
            await InitialiseViewModel((TViewModel)viewModel);
        }


        public Func<TViewModel, CancelEventArgs, Task> AboutToChangeFromViewModel { get; set; }

        public async Task InvokeAboutToChangeFromViewModel(INotifyPropertyChanged viewModel, CancelEventArgs cancel)
        {
            if (AboutToChangeFromViewModel == null) return;

            "Invoking AboutToChangeFromViewModel".Log();
            await AboutToChangeFromViewModel((TViewModel)viewModel, cancel);
        }

        public Func<TViewModel, Task> ChangingFromViewModel { get; set; }

        public async Task InvokeChangingFromViewModel(INotifyPropertyChanged viewModel)
        {
            if (ChangingFromViewModel == null) return;

            "Invoking ChangingFromViewModel".Log();
            await ChangingFromViewModel((TViewModel)viewModel);
        }


        public Func<TViewModel, Task> ChangedToViewModel { get; set; }

        public async Task InvokeChangedToViewModel(INotifyPropertyChanged viewModel)
        {
            if (ChangedToViewModel == null) return;

            "Invoking ChangedToViewModel".Log();
            await ChangedToViewModel((TViewModel)viewModel);
        }

        public Func<TViewModel,string, Task> ChangedToWithDataViewModel { get; set; }

        public async Task InvokeChangedToWithDataViewModel(INotifyPropertyChanged viewModel, string data)
        {
            if (ChangedToWithDataViewModel == null) return;

            "Invoking ChangedToWithDataViewModel".Log();
            await ChangedToWithDataViewModel((TViewModel)viewModel,data);
        }

        public ViewModelStateDefinition(TState state) : base(state)
        {
        }
    }
}
