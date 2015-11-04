using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BuildIt.Lifecycle.States;
using BuildIt.Lifecycle.States.ViewModel;

namespace BuildIt.Lifecycle
{
    public class WindowManager
    {
        private IRegionManager RegionManager { get; }

        private IDictionary<string, CoreWindow> Windows { get; }=new Dictionary<string, CoreWindow>(); 

        public WindowManager(IHasRegionManager root)
        {
            RegionManager = root.RegionManager;
            RegionManager.RegionCreated += RegionManager_RegionCreated;
            RegionManager.RegionIsClosing += RegionManager_RegionIsClosing;
        }

        private void RegionManager_RegionIsClosing(object sender, ParameterEventArgs<IApplicationRegion> e)
        {
            var view = Windows.SafeDictionaryValue<string, CoreWindow, CoreWindow>(e.Parameter1.RegionId);
            view.Close();

        }

#pragma warning disable 1998 // Async required for Windows UWP support for multiple views
        private async void RegionManager_RegionCreated(object sender, ParameterEventArgs<IApplicationRegion> e)
#pragma warning restore 1998
        {
#if WINDOWS_UWP
            var newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var frame = new Frame();

                var region = e.Parameter1;

                var interfaces = region.GetType().GetInterfaces();
                foreach (var it in interfaces)
                {
                    if (it.IsConstructedGenericType && it.GetGenericTypeDefinition() == typeof (IHasStateManager<,>))
                    {
                        var args = it.GenericTypeArguments;
                        var fnt = typeof (FrameNavigation<,>).MakeGenericType(args);
                        var fn = Activator.CreateInstance(fnt, frame, region, string.Empty);
                    }
                }


                await region.Startup(sender as IRegionManager);
                Window.Current.Content = frame;

                region.CloseRegion += Region_CloseRegion;

                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;

                Windows[region.RegionId] = Window.Current.CoreWindow;
            });


            var viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
            Debug.WriteLine(viewShown);
#endif
        }

        private void Region_CloseRegion(object sender, EventArgs e)
        {
            Windows[(sender as IApplicationRegion).RegionId].Close();
        }
    }

    public static class NavigationHelper
    {
        private static IDictionary<string, Type> NavigationIndex { get; } = new Dictionary<string, Type>();

        public static void Register<TStateInfo, TPage>(TStateInfo state)
                    where TStateInfo : struct

        {
            var key = KeyFromState<TStateInfo>(state);
            NavigationIndex[key] = typeof(TPage);
        }

        private static string KeyFromState<TStateInfo>(TStateInfo state)
        {
            var type = state.GetType().Name;
            return type + state.ToString();
        }


        public static Type TypeForState<TStateInfo>(TStateInfo state)
        {
            return NavigationIndex[KeyFromState(state)];

        }
    }

    public class FrameNavigation<TState,TTransition>:IHasCurrentViewModel 
        where TState : struct
        where TTransition:struct
    {
        public INotifyPropertyChanged CurrentViewModel => StateManager?.CurrentViewModel;


        public IViewModelStateManager<TState, TTransition> StateManager { get; }

        private Frame RootFrame { get; }

        public FrameNavigation(Frame rootFrame,
            IHasViewModelStateManager<TState, TTransition> hasStateManager,
            string registerAs = null)
        {
            var stateManager = hasStateManager.StateManager;
            if (string.IsNullOrWhiteSpace( registerAs ))
            {
                registerAs = hasStateManager.GetType().Name;
            }
            Application.Current.Resources[registerAs] = this;
            RootFrame = rootFrame;
            RootFrame.Tag = registerAs;
            StateManager = stateManager;
            StateManager.StateChanged += StateManager_StateChanged;
        }

        private void StateManager_StateChanged(object sender, StateEventArgs<TState> e)
        {
            var tp = NavigationHelper.TypeForState(e.State);
            if (RootFrame.BackStack.FirstOrDefault()?.SourcePageType == tp)
            {
                RootFrame.GoBack();
            }
            else
            {
                RootFrame.Navigate(tp);
            }
        }


    }
}