using BuildIt.Lifecycle.Interfaces;
using BuildIt.Lifecycle.Regions;
using BuildIt.Lifecycle.States;
using BuildIt.States.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace BuildIt.Lifecycle
{
    /// <summary>
    /// Handles navigation within a frame
    /// </summary>
    public class FrameNavigation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameNavigation"/> class.
        /// </summary>
        /// <param name="rootFrame">The frame where navigation should occur</param>
        /// <param name="stateGroup">The state group</param>
        public FrameNavigation(
            Frame rootFrame,
                IStateGroup stateGroup)
        // ,string registerAs = null)
        {
            // var stateManager = hasStateManager.StateManager;
            // if (string.IsNullOrWhiteSpace( registerAs ))
            // {
            //    registerAs = hasStateManager.GetType().Name;
            // }
            // Application.Current.Resources[registerAs] = this;

            // Capture local references to arguments
            RootFrame = rootFrame;
            StateGroup = stateGroup;

            if (!string.IsNullOrWhiteSpace(StateGroup.CurrentStateName))
            {
                HandleState(StateGroup.GroupName, StateGroup.CurrentStateName, true);
            }

            // Handle state change - this will trigger navigation
            StateGroup.StateChanged += StateManager_StateChanged;

            RootFrame.Navigated += RootFrame_Navigated;
            RootFrame.Navigating += RootFrame_Navigating;

            // RootFrame.Tag = registerAs;
        }

        /// <summary>
        /// Gets the current state data
        /// </summary>
        private INotifyPropertyChanged CurrentStateData => StateGroup?.CurrentStateData;

        /// <summary>
        /// Gets the state group
        /// </summary>
        private IStateGroup StateGroup { get; }

        private Frame RootFrame { get; }

        private void HandleState(string groupName, string stateName, bool isNewState)

        {
            // Retrieve the view that was registered for the new state
            var tp = NavigationHelper.TypeForName(groupName, stateName);
            if (tp == null)
            {
                $"Unable to find type of view to navigate to [{stateName}]".LogLifecycleInfo();
                return;
            }

            // Check if this is a forward navigation (ie new state)
            if (isNewState)
            {
                // Navigate to the new type of view
                // Pass through the new state data
                RootFrame.Navigate(tp, CurrentStateData);
            }
            else
            {
                // Remove items off the back stack until we find the view
                // we're after
                var previous = RootFrame.BackStack.FirstOrDefault();
                while (previous != null && previous.SourcePageType != tp)
                {
                    RootFrame.BackStack.Remove(previous);
                    previous = RootFrame.BackStack.FirstOrDefault();
                }

                if (previous != null)
                {
                    RootFrame.GoBack();
                }
            }
        }

        private void StateManager_StateChanged(object sender, IStateEventArgs e)
        {
            using (e.Defer())
            {
                HandleState(StateGroup.GroupName, e.StateName, e.IsNewState);
            }
        }

        private void RootFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            // Retrieve the navigation parameter, which is the view model that
            // should be set as the data context
            var viewModel = e.Parameter as INotifyPropertyChanged;
            $"Found data context? '{viewModel != null}'".LogLifecycleInfo();
            if (viewModel == null)
            {
                return;
            }

            // Retrieve the root content
            var pg = RootFrame.Content as FrameworkElement;
            if (pg == null)
            {
                return;
            }

            // Set the data context to be the view model
            pg.DataContext = viewModel;

            // ReSharper disable once SuspiciousTypeConversion.Global - this is possible if page or control implements IHasStates
            if (pg is IHasStates viewHasStates && viewModel is IHasStates vmHasStates)
            {
                viewHasStates.StateManager.Bind(vmHasStates.StateManager);
            }

            if (viewModel is IHasRegionManager regions)
            {
                foreach (var region in regions.RegionManager.CurrentRegions.OfType<ISingleAreaApplicationRegion>())
                {
                    var frame = VisualRegion.NamedRegions.SafeValue(region.RegionId) as Frame;
                    if (frame != null)
                    {
                        var nav = new FrameNavigation(frame, region.AreaStateGroup);
                        if (pg is IHasStates pageState)
                        {
                            pageState.StateManager.Bind(region.StateManager);
                        }
                    }
                }
            }

            // var pgHasNotifier = pg as IHasStates;
            // if (pgHasNotifier == null) return;
            //var sm = (dc as IHasStates)?.StateManager;
            //if (sm != null)
            //{
            //    var groups = sm.StateGroups;
            //    var inotifier = typeof(EnumStateGroup<>);
            //    var vsct = typeof(VisualStateChanger<>);
            //    foreach (var stateGroup in groups)
            //    {
            //        var typeArg = stateGroup.Value.GetType().GenericTypeArguments.FirstOrDefault();
            //        var groupNotifier = inotifier.MakeGenericType(typeArg);
            //        if (stateGroup.Value.GetType().GetTypeInfo().ImplementedInterfaces.Contains(groupNotifier))
            //        {
            //            var vsc = Activator.CreateInstance(vsct.MakeGenericType(typeArg), pg, stateGroup.Value);
            //        }
            //    }
            //}

            // var pps = dc.GetType().GetTypeInfo().DeclaredProperties;
            // "Iterating through declared properties".Log();
            // foreach (var p in pps)
            // {
            //    var pt = p.PropertyType.GetTypeInfo();
            //    var interfaces = pt.ImplementedInterfaces;
            //    if (pt.IsInterface)
            //    {
            //        interfaces = new[] { pt.AsType() }.Union(interfaces);
            //    }
            //    "Completed interface search".Log();
            //    var ism = typeof(IStateManager<,>);
            //    var vsct = typeof(VisualStateChanger<,>);
            //    foreach (var inf in interfaces)
            //    {
            //        $"Inspecting interface {inf.Name}".Log();
            //        if (inf.IsConstructedGenericType &&
            //            inf.GetGenericTypeDefinition() == ism)
            //        {
            //            "Interface matched, creating instance".Log();
            //            var parm = inf.GenericTypeArguments;
            //            var vsc = Activator.CreateInstance(vsct.MakeGenericType(parm), pg, p.GetValue(dc));
            //            "Instance created".Log();
            //        }
            //    }
            // }
        }

        private void RootFrame_Navigating(object sender, Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e)
        {
        }
    }

    public static class UIHelper
    {
        public static T AdvancedFindControlByType<T>(this DependencyObject container, string name = null)
            where T : DependencyObject
        {
            if (container is T match &&
                (container.GetValue(FrameworkElement.NameProperty)
                     .Equals(name) || name == null))
            {
                return match;
            }

            T t = default(T);
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(container); i++)
            {
                var child = VisualTreeHelper.GetChild(container, i);
                t = child.AdvancedFindControlByType<T>(name);
                if (t != null)
                {
                    break;
                }
            }

            if (t == null && container is FrameworkElement fe)
            {
                var cp = container.GetType().GetTypeInfo().CustomAttributes//.GetCustomAttribute<ContentPropertyAttribute>(true);
                                   .FirstOrDefault(x => x.AttributeType == typeof(ContentPropertyAttribute));// GetCustomAttribute<ContentPropertyAttribute>();
                if (cp != null)
                {
                    var content = container.GetType().GetProperty("Content").GetValue(fe) as DependencyObject;
                    if (content != null)
                    {
                        t = content.AdvancedFindControlByType<T>(name);
                    }
                }
            }
            return t;
        }
    }

    public static class VisualRegion
    {
        public static string GetRegionName(DependencyObject entity)
        {
            return entity.GetValue(RegionNameProperty) as string;
        }

        public static void SetRegionName(DependencyObject entity, string value)
        {
            entity.SetValue(RegionNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RegionNameProperty =
            DependencyProperty.Register("RegionName", typeof(string), typeof(VisualRegion), new PropertyMetadata(null, PropertyChangedCallback));

        public static IDictionary<string, DependencyObject> NamedRegions { get; } = new Dictionary<string, DependencyObject>();

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var name = dependencyPropertyChangedEventArgs.NewValue as string;
            NamedRegions[name] = dependencyObject;
        }
    }
}