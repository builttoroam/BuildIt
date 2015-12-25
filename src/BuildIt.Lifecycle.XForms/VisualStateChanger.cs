using BuildIt.Lifecycle.States;
using BuildIt.States;

namespace BuildIt.Lifecycle
{
    public class VisualStateChanger<TState>
        where TState : struct
    {
        public INotifyStateChanged<TState> ChangeNotifier { get; }

        private IStateManager VisualStateManager { get; }

        public VisualStateChanger(IHasStates visualStateRoot, INotifyStateChanged<TState> changeNotifier)
        {
            VisualStateManager = visualStateRoot.StateManager;
            ChangeNotifier = changeNotifier;
            ChangeNotifier.StateChanged += StateManager_StateChanged;
        }

        private void StateManager_StateChanged(object sender, StateEventArgs<TState> e)
        {
            VisualStateManager.GoToState(e.State, e.UseTransitions);
        }
    }
}