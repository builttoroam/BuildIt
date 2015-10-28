using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public interface IViewModelTransitionDefinition<TState>: ITransitionDefinition<TState>
        where TState : struct
    {
        Func<TState, INotifyPropertyChanged, CancelEventArgs, Task> LeavingStateViewModel { get; set; }

        Func<TState, INotifyPropertyChanged, Task> ArrivedStateViewModel { get; set; }

    }
}