using BuildIt.Lifecycle.States;
using BuildIt.States;
using Xamarin.Forms;

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

    public static class LifecycleHelper
    {

        public static NavigationRegistrationHelper RegisterView<TView>() where TView : Page
        {
            return new NavigationRegistrationHelper { ViewType = typeof(TView) };
        }


    }
}