using System;
using System.ComponentModel;
using System.Threading.Tasks;
using BuildIt.States;
using BuildIt.States.Interfaces;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public interface IViewModelStateDefinition<TState,TViewModel> 
        :IGenerateViewModel, 
        IEnumStateDefinition<TState> 
        where TViewModel: INotifyPropertyChanged 
        where TState : struct
    {
        Func<TViewModel, Task> InitialiseViewModel { get; set; }

        Func<TViewModel, CancelEventArgs, Task> AboutToChangeFromViewModel { get; set; }

    
        Func<TViewModel, Task> ChangingFromViewModel { get; set; }

        Func<TViewModel, Task> ChangedToViewModel { get; set; }

        Func<TViewModel,string, Task> ChangedToWithDataViewModel { get; set; }
    }
}