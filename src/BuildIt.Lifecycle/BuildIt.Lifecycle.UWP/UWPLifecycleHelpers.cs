using BuildIt.States.Interfaces;
using BuildIt.States.Typed.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BuildIt.Lifecycle
{
    /// <summary>
    /// Helpers for working with UWP Lifecycle
    /// </summary>
    public static class UWPLifecycleHelpers
    {
        /// <summary>
        /// Binds the page to states
        /// </summary>
        /// <param name="rootPage">The page</param>
        /// <param name="hasManager">The reference to state manager</param>
        /// <returns>Binder object</returns>
        public static IStateBinder Bind(this Page rootPage, IHasStates hasManager)
        {
            return new VisualStateManagerBinder(rootPage, hasManager.StateManager);
        }

        private class VisualStateManagerBinder : IStateBinder
        {
            public VisualStateManagerBinder(Page rootPage, IStateManager manager)
            {
                StateManagerToBindTo = manager;

                var groups = VisualStateManager.GetVisualStateGroups(rootPage.Content as FrameworkElement);

                var inotifier = typeof(EnumStateGroup<>);
                var vsct = typeof(VisualStateChanger<>);

                foreach (var kvp in manager.StateGroups)
                {
                    var sg = groups.FirstOrDefault(grp => grp.Name == kvp.Key);
                    if (sg == null)
                    {
                        continue;
                    }

                    var typeArg = kvp.Value.GetType().GenericTypeArguments.FirstOrDefault();

                    var groupNotifier = inotifier.MakeGenericType(typeArg);
                    if (kvp.Value.GetType().GetTypeInfo().ImplementedInterfaces.Contains(groupNotifier))
                    {
                        var vsc = Activator.CreateInstance(vsct.MakeGenericType(typeArg), rootPage, kvp.Value) as IStateBinder;
                        GroupBinders.Add(vsc);
                    }
                }
            }

            private IList<IStateBinder> GroupBinders { get; } = new List<IStateBinder>();

            private IStateManager StateManagerToBindTo { get; }

            public async Task Bind()
            {
                foreach (var groupBinder in GroupBinders)
                {
                    await groupBinder.Bind();
                }
            }

            public void Dispose()
            {
                Unbind();
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