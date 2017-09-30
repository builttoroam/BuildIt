using BuildIt.States.Interfaces;

namespace BuildIt.Lifecycle
{
    /// <summary>
    /// Helper class to handle changing visual states
    /// </summary>
    /// <typeparam name="TState">The type of state</typeparam>
    public class VisualStateChanger<TState>
        where TState : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualStateChanger{TState}"/> class.
        /// </summary>
        /// <param name="visualStateRoot">The element that has visual states</param>
        /// <param name="changeNotifier">The state group</param>
        public VisualStateChanger(IHasStates visualStateRoot, INotifyTypedStateChange<TState> changeNotifier)
        {
            VisualStateManager = visualStateRoot.StateManager;
            ChangeNotifier = changeNotifier;
            ChangeNotifier.TypedStateChanged += StateManager_StateChanged;
        }

        /// <summary>
        /// Gets the state group
        /// </summary>
        public INotifyTypedStateChange<TState> ChangeNotifier { get; }

        private IStateManager VisualStateManager { get; }

        private void StateManager_StateChanged(object sender, ITypedStateEventArgs<TState> e)
        {
            VisualStateManager.GoToState(e.TypedState, e.UseTransitions);
        }
    }
}