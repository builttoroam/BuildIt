using BuildIt.Lifecycle.States;
using BuildIt.States;
using BuildIt.States.Interfaces;
using BuildIt.States.Typed;
using Xamarin.Forms;

namespace BuildIt.Lifecycle
{
    public class VisualStateChanger<TState>
        where TState : struct
    {
        public INotifyTypedStateChange<TState> ChangeNotifier { get; }

        private IStateManager VisualStateManager { get; }

        public VisualStateChanger(IHasStates visualStateRoot, INotifyTypedStateChange<TState> changeNotifier)
        {
            VisualStateManager = visualStateRoot.StateManager;
            ChangeNotifier = changeNotifier;
            ChangeNotifier.TypedStateChanged += StateManager_StateChanged;
        }

        private void StateManager_StateChanged(object sender, ITypedStateEventArgs<TState> e)
        {
            VisualStateManager.GoToState(e.TypedState, e.UseTransitions);
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