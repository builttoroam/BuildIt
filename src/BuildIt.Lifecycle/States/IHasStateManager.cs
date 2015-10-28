namespace BuildIt.Lifecycle.States
{
    public interface IHasStateManager<TState,TTransition>
        where TState:struct
        where TTransition:struct
    {
        IStateManager<TState, TTransition> StateManager { get; } 
    }

    public interface IHasRegionManager
    {
        IRegionManager RegionManager { get; }
    }
}