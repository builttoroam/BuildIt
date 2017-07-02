using BuildIt.Lifecycle.States;
using BuildIt.States;
using BuildIt.States.Interfaces;
using Xamarin.Forms;

namespace BuildIt.Lifecycle
{
    public class VisualStateChanger<TState>
        where TState : struct
    {
        public INotifyEnumStateChanged<TState> ChangeNotifier { get; }

        private IStateManager VisualStateManager { get; }

        public VisualStateChanger(IHasStates visualStateRoot, INotifyEnumStateChanged<TState> changeNotifier)
        {
            VisualStateManager = visualStateRoot.StateManager;
            ChangeNotifier = changeNotifier;
            ChangeNotifier.EnumStateChanged += StateManager_StateChanged;
        }

        private void StateManager_StateChanged(object sender, EnumStateEventArgs<TState> e)
        {
            VisualStateManager.GoToState(e.EnumState, e.UseTransitions);
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