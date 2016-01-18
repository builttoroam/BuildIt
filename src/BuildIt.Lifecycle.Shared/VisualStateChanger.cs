using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BuildIt.Lifecycle.States;
using BuildIt.States;

namespace BuildIt.Lifecycle
{
    public class VisualStateChanger<TState>:IStateBinder
        where TState : struct
    {
        public INotifyStateChanged<TState> ChangeNotifier { get; }

        private Control VisualStateRoot { get; }

        public VisualStateChanger(Control visualStateRoot, INotifyStateChanged<TState> changeNotifier)
        {
            VisualStateRoot = visualStateRoot;
            ChangeNotifier = changeNotifier;
            ChangeNotifier.StateChanged += StateManager_StateChanged;

            var control = (VisualStateRoot as UserControl)?.Content as FrameworkElement;
            if (control == null)
            {
                return;
            }

            var stateTypeName = typeof (TState).Name;
            var visualStateGroup =
                VisualStateManager.GetVisualStateGroups(control).FirstOrDefault(x => x.Name == stateTypeName);

            if (visualStateGroup == null) return;
            visualStateGroup.CurrentStateChanged += VisualStateGroupOnCurrentStateChanged;
        }

        private void VisualStateGroupOnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            var newState = e.NewState?.Name.EnumParse<TState>()??default(TState);
            if (newState.Equals(default(TState))) return;
            ChangeNotifier?.ChangeTo(newState, false);
        }

        private void StateManager_StateChanged(object sender, StateEventArgs<TState> e)
        {
            VisualStateManager.GoToState(VisualStateRoot, e.State.ToString(), e.UseTransitions);
        }

        public void Unbind()
        {
            ChangeNotifier.StateChanged -= StateManager_StateChanged;
        }
    }

    public static class LifecycleHelper
    {

        public static NavigationRegistrationHelper RegisterView<TView>() where TView:Page
        {
            return new NavigationRegistrationHelper {ViewType = typeof (TView)};
        }


    }

    public static class UWPLifecycleHelpers
    {
       


        public static IStateBinder Bind(this Page rootPage, IHasStates hasManager)
        {
            return new VisualStateManagerBinder(rootPage,hasManager.StateManager);
        }
        
        private class VisualStateManagerBinder : IStateBinder
        {
            IStateManager StateManagerToBindTo { get; }

            IList<IStateBinder> GroupBinders { get; } = new List<IStateBinder>();
            public VisualStateManagerBinder(Page rootPage, IStateManager manager)
            {
                StateManagerToBindTo = manager;

                var groups = VisualStateManager.GetVisualStateGroups(rootPage.Content as FrameworkElement);

                var inotifier = typeof(INotifyStateChanged<>);
                var vsct = typeof(VisualStateChanger<>);

                foreach (var kvp in manager.StateGroups)
                {
                    var sg = groups.FirstOrDefault(grp => grp.Name == kvp.Key.Name);
                    if (sg == null) continue;

                    var groupNotifier = inotifier.MakeGenericType(kvp.Key);
                    if (kvp.Value.GetType().GetTypeInfo().ImplementedInterfaces.Contains(groupNotifier))
                    {
                        var vsc = Activator.CreateInstance(vsct.MakeGenericType(kvp.Key), rootPage, kvp.Value) as IStateBinder;
                        GroupBinders.Add(vsc);
                    }
                }
            }

            public void Unbind()
            {
                foreach (var groupBinder in GroupBinders)
                {
                    groupBinder.Unbind();
                }
            }
        }


    }


}