using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BuildIt.Lifecycle.States;

namespace BuildIt.Lifecycle
{
    public class VisualStateChanger<TState, TTransition>
        where TState : struct
        where TTransition : struct
    {
        public IStateManager<TState, TTransition> StateManager { get; }

        private Control VisualStateRoot { get; }

        public VisualStateChanger(Control visualStateRoot, IStateManager<TState, TTransition> stateManager)
        {
            VisualStateRoot = visualStateRoot;
            StateManager = stateManager;
            StateManager.StateChanged += StateManager_StateChanged;
        }

        private void StateManager_StateChanged(object sender, StateEventArgs<TState> e)
        {
            VisualStateManager.GoToState(VisualStateRoot, e.State.ToString(), e.UseTransitions);
        }
    }
}