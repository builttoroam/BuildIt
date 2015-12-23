using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Practices.ServiceLocation;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public class ViewModelStateDefinition<TState, TViewModel> :
        StateDefinition<TState>,
        IViewModelStateDefinition<TState, TViewModel>
        where TState : struct
        where TViewModel : INotifyPropertyChanged 
    {
        public Type ViewModelType => typeof(TViewModel);

        public async Task<INotifyPropertyChanged> Generate(IContainer container)
        {
            $"Creating instance of {typeof(TViewModel).Name}".Log();
            var vm = ServiceLocator.Current.GetInstance<TViewModel>();

            "Registering dependencies".Log();
            (vm as ICanRegisterDependencies)?.RegisterDependencies(container);

            if (InitialiseViewModel != null)
            {
                "Initialising ViewModel".Log();
                await InitialiseViewModel(vm);
            }

            "ViewModel generated".Log();
            return vm;
        }

        public Func<TViewModel, Task> InitialiseViewModel { get; set; }

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
    }
}
