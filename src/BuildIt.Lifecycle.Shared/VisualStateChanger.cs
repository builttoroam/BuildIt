using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BuildIt.Lifecycle.States;

namespace BuildIt.Lifecycle
{
    public class VisualStateChanger<TState>
        where TState : struct
    {
        public INotifyStateChanged<TState> ChangeNotifier { get; }

        private Control VisualStateRoot { get; }

        public VisualStateChanger(Control visualStateRoot, INotifyStateChanged<TState> changeNotifier)
        {
            VisualStateRoot = visualStateRoot;
            ChangeNotifier = changeNotifier;
            ChangeNotifier.StateChanged += StateManager_StateChanged;
        }

        private void StateManager_StateChanged(object sender, StateEventArgs<TState> e)
        {
            VisualStateManager.GoToState(VisualStateRoot, e.State.ToString(), e.UseTransitions);
        }
    }
}