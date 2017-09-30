using BuildIt.States.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BuildIt.Lifecycle
{
    /// <summary>
    /// Handles changing the visual state on a page based on a change in state
    /// </summary>
    /// <typeparam name="TState">The type (enum) of states</typeparam>
    public class VisualStateChanger<TState> : IStateBinder
                where TState : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualStateChanger{TState}"/> class.
        /// </summary>
        /// <param name="visualStateRoot">The control that hosts the visual states</param>
        /// <param name="changeNotifier">The state group</param>
        public VisualStateChanger(Control visualStateRoot, INotifyTypedStateChange<TState> changeNotifier)
        {
            VisualStateRoot = visualStateRoot;
            ChangeNotifier = changeNotifier;

            var control = (VisualStateRoot as UserControl)?.Content as FrameworkElement;
            if (control == null)
            {
                return;
            }

            var stateTypeName = typeof(TState).Name;
            var visualStateGroup =
                VisualStateManager.GetVisualStateGroups(control).FirstOrDefault(x => x.Name == stateTypeName);

            if (visualStateGroup == null)
            {
                return;
            }

            visualStateGroup.CurrentStateChanged += VisualStateGroupOnCurrentStateChanged;
        }

        /// <summary>
        /// Gets the state group
        /// </summary>
        public INotifyTypedStateChange<TState> ChangeNotifier { get; }

        private Control VisualStateRoot { get; }

        /// <summary>
        /// Binds the state group
        /// </summary>
        /// <returns>Task to await</returns>
        public Task Bind()
        {
            ChangeNotifier.TypedStateChanged += StateManager_StateChanged;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Disposes and unbinds
        /// </summary>
        public void Dispose()
        {
            Unbind();
        }

        /// <summary>
        /// Unbinds the state group
        /// </summary>
        public void Unbind()
        {
            ChangeNotifier.TypedStateChanged -= StateManager_StateChanged;
        }

        private void StateManager_StateChanged(object sender, ITypedStateEventArgs<TState> e)
        {
            VisualStateManager.GoToState(VisualStateRoot, e.TypedState.ToString(), e.UseTransitions);
        }

        private void VisualStateGroupOnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            // var newState = e.NewState?.Name.EnumParse<TState>()??default(TState);
            // if (newState.Equals(default(TState))) return;
            (ChangeNotifier as IStateGroup)?.ChangeToStateByName(e.NewState?.Name, false);
        }
    }
}