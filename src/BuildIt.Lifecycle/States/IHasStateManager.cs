using BuildIt.Lifecycle.States.ViewModel;

namespace BuildIt.Lifecycle.States
{
    public interface IHasStateManager<TState,TTransition>
        where TState:struct
        where TTransition:struct
    {
        IStateManager<TState, TTransition> StateManager { get; } 
    }

    public interface IHasViewModelStateManager<TState, TTransition>
        where TState : struct
        where TTransition : struct
    {
        IViewModelStateManager<TState, TTransition> StateManager { get; }
    }

    public interface IHasRegionManager
    {
        IRegionManager RegionManager { get; }
    }
}