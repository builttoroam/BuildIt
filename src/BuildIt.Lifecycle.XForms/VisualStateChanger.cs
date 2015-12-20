using BuildIt.Lifecycle.States;
using BuildIt.VisualStates;

namespace BuildIt.Lifecycle
{
    public class VisualStateChanger<TState, TTransition>
        where TState : struct
        where TTransition : struct
    {
        public IStateManager<TState, TTransition> StateManager { get; }

        private IVisualStateManager VisualStateManager { get; }

        public VisualStateChanger(IHasVisualStateManager visualStateRoot, IStateManager<TState, TTransition> stateManager)
        {
            VisualStateManager = visualStateRoot.VisualStateManager;
            StateManager = stateManager;
            StateManager.StateChanged += StateManager_StateChanged;
        }

        private void StateManager_StateChanged(object sender, StateEventArgs<TState> e)
        {
            VisualStateManager.GoToState(e.State, e.UseTransitions);
        }
    }
}