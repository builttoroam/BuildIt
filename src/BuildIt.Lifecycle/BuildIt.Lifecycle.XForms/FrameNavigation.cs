using BuildIt.Lifecycle.States;
using BuildIt.States.Interfaces;
using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;

namespace BuildIt.Lifecycle
{
    /// <summary>
    /// Handles navigation within a frame for a type of states
    /// </summary>
    /// <typeparam name="TState">The type (enum) of states</typeparam>
    public class FrameNavigation<TState>
        where TState : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameNavigation{TState}"/> class.
        /// </summary>
        /// <param name="navigationRoot">The page where navigation will take place</param>
        /// <param name="sm">The state manager that defines the states</param>
        public FrameNavigation(
            NavigationPage navigationRoot,
            IStateManager sm)
        // ,string registerAs = null)
        {
            StateManager = sm;
            var stateNotifier = sm.TypedStateGroup<TState>();
            // var stateManager = hasStateManager.StateManager;
            // if (string.IsNullOrWhiteSpace( registerAs ))
            // {
            //    registerAs = hasStateManager.GetType().Name;
            // }
            // Application.Current.Resources[registerAs] = this;
            NavigationRoot = navigationRoot;

            // RootFrame.Navigated += RootFrame_Navigated;
            // RootFrame.Navigating += RootFrame_Navigating;
            NavigationRoot.Pushed += NavigationRoot_Pushed;

            // RootFrame.Tag = registerAs;
            StateNotifier = stateNotifier;
            StateNotifier.TypedStateChanged += StateManager_StateChanged;

            sm.GoToPreviousStateIsBlockedChanged += Sm_GoToPreviousStateIsBlockedChanged;
        }

        /// <summary>
        /// Gets the current state data
        /// </summary>
        public INotifyPropertyChanged CurrentStateData => (StateNotifier as IStateGroup)?.CurrentStateData;

        /// <summary>
        /// Gets the current state
        /// </summary>
        public INotifyTypedStateChange<TState> StateNotifier { get; }

        private NavigationPage NavigationRoot { get; }

        private IStateManager StateManager { get; }

        // private void RootFrame_Navigating(object sender, Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e)
        // {
        // }
        private void NavigationRoot_Pushed(object sender, NavigationEventArgs e)
        // private void RootFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            // var dc = e.Parameter as INotifyPropertyChanged;
            // $"Found data context? '{dc != null}'".LogLifecycleInfo();
            // if (dc == null) return;
            var pg = e.Page;
            if (pg == null)
            {
                return;
            }

            var dc = pg.BindingContext as INotifyPropertyChanged;
            $"Found data context? '{dc != null}'".LogLifecycleInfo();
            if (dc == null)
            {
                return;
            }

            var pgHasNotifier = pg as IHasStates;
            if (pgHasNotifier == null)
            {
                return;
            }

            var sm = (dc as IHasStates)?.StateManager;

            // pgHasNotifier.StateManager.Bind(sm);
            var groups = sm.StateGroups;
            var inotifier = typeof(INotifyTypedStateChange<>);
            var vsct = typeof(VisualStateChanger<>);
            foreach (var stateGroup in groups)
            {
                // TODO: FIX

                // var groupNotifier = inotifier.MakeGenericType(stateGroup.Key);
                // if (stateGroup.Value.GetType().GetTypeInfo().ImplementedInterfaces.Contains(groupNotifier))
                // {
                //    var vsc = Activator.CreateInstance(vsct.MakeGenericType(stateGroup.Key), pgHasNotifier, stateGroup.Value);
                // }
            }

            // var pps = dc.GetType().GetTypeInfo().DeclaredProperties;
            // "Iterating through declared properties".LogLifecycleInfo();
            // foreach (var p in pps)
            // {
            //    var pt = p.PropertyType.GetTypeInfo();
            //    var interfaces = pt.ImplementedInterfaces;
            //    if (pt.IsInterface)
            //    {
            //        interfaces = new[] { pt.AsType() }.Union(interfaces);
            //    }
            //    "Completed interface search".LogLifecycleInfo();
            //    var ism = typeof(IStateManager<,>);
            //    //var vsct = typeof(VisualStateChanger<,>);
            //    foreach (var inf in interfaces)
            //    {
            //        $"Inspecting interface {inf.Name}".LogLifecycleInfo();
            //        if (inf.IsConstructedGenericType &&
            //            inf.GetGenericTypeDefinition() == ism)
            //        {
            //            "Interface matched, creating instance".LogLifecycleInfo();
            //            var parm = inf.GenericTypeArguments;
            //            var vsc = Activator.CreateInstance(vsct.MakeGenericType(parm), pg, p.GetValue(dc));
            //            "Instance created".LogLifecycleInfo();
            //        }
            //    }
            // }
        }

        private void Sm_GoToPreviousStateIsBlockedChanged(object sender, EventArgs e)
        {
            UpdateNavigationBar();
        }

        private async void StateManager_StateChanged(object sender, ITypedStateEventArgs<TState> e)
        {
            var tp = NavigationHelper.TypeForState(e.TypedState);

            if (e.IsNewState)
            {
                var page = Activator.CreateInstance(tp) as Page;
                page.BindingContext = CurrentStateData;
                await NavigationRoot.Navigation.PushAsync(page);
            }
            else
            {
                var previous = NavigationRoot.Navigation.NavigationStack.FirstOrDefault();
                while (previous != null && previous.GetType() != tp)
                {
                    await NavigationRoot.Navigation.PopAsync();
                    previous = NavigationRoot.Navigation.NavigationStack.FirstOrDefault();
                }

                if (previous != null)
                {
                    await NavigationRoot.Navigation.PopAsync();
                }
            }

            UpdateNavigationBar();
        }

        private void UpdateNavigationBar()
        {
            if (StateManager.PreviousStateExists && !StateManager.GoToPreviousStateIsBlocked)
            {
                NavigationPage.SetHasBackButton(NavigationRoot.CurrentPage, true);
            }
            else
            {
                NavigationPage.SetHasBackButton(NavigationRoot.CurrentPage, false);
            }
        }
    }

    // public interface ICodeBehindViewModel<TViewModel>
    // {
    //    Wrapper<TViewModel> Data { get; }
    // }

    // public class Wrapper<T> : NotifyBase
    // {
    //    private T viewModel;

    // public T ViewModel
    //    {
    //        get { return viewModel; }
    //        private set
    //        {
    //            viewModel = value;
    //            OnPropertyChanged();
    //        }
    //    }

    // public Wrapper(FrameworkElement element)
    //    {
    //        element.DataContextChanged += Element_DataContextChanged;
    //    }

    // private void Element_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    //    {
    //        if (args.NewValue is T)
    //        {
    //            ViewModel = (T)args.NewValue;
    //        }
    //        else
    //        {
    //            ViewModel = default(T);
    //        }
    //    }
    // }
}