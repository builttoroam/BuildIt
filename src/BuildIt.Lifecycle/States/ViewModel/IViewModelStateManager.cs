using System.ComponentModel;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public interface IViewModelStateManager<TState, TTransition> :
        ICanRegisterDependencies,
        IStateManager<TState, TTransition>,
        IHasCurrentViewModel, IRegisterForUIAccess
        where TState : struct
        where TTransition : struct
    {
        IViewModelStateDefinition<TState, TViewModel> DefineViewModelState<TViewModel>(TState state)
            where TViewModel : INotifyPropertyChanged;

        IViewModelStateDefinition<TState, TViewModel> DefineViewModelState<TViewModel>(
            IViewModelStateDefinition<TState, TViewModel> stateDefinition)
            where TViewModel : INotifyPropertyChanged;
        
    }
}