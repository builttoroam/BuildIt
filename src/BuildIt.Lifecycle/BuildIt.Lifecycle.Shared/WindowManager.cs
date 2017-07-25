using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BuildIt.Lifecycle.States;
using BuildIt.States;
using BuildIt.States.Interfaces;
#if WINDOWS_UWP
using Windows.Phone.UI.Input;
#endif
namespace BuildIt.Lifecycle
{
    public class WindowManager
    {
        private IRegionManager RegionManager { get; }

        private IDictionary<string, CoreWindow> RegionWindows { get; }=new Dictionary<string, CoreWindow>(); 

        public WindowManager(IHasRegionManager root)
        {
            RegionManager = root.RegionManager;
            RegionManager.RegionCreated = RegionManager_RegionCreated;
            RegionManager.RegionIsClosing = RegionManager_RegionIsClosing;
        }

        private void RegionManager_RegionIsClosing(IRegionManager sender, IApplicationRegion e)
        {
            var view = RegionWindows.SafeValue<string, CoreWindow, CoreWindow>(e.RegionId);
            view.Close();

        }

#pragma warning disable 1998 // Async required for Windows UWP support for multiple views
        private async void RegionManager_RegionCreated(IRegionManager sender, IApplicationRegion e)
#pragma warning restore 1998
        {
#if WINDOWS_UWP

            var isPrimary = RegionManager.IsPrimaryRegion(e);
            var newView = isPrimary
                ? CoreApplication.MainView
                : CoreApplication.CreateNewView();
            var newViewId = 0;
            var context = new UniversalUIContext(newView.Dispatcher);
            e.RegisterForUIAccess(context);



            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,  () =>
            {
                var frame = new Frame();

                var region = e;

                var hasStates = region as IHasStates;
                var sm = (region as IHasStates)?.StateManager;
                if (sm != null)
                {

                    var groups = sm.StateGroups;
                    var inotifier = typeof(INotifyTypedStateChanged<>);
                    foreach (var stateGroup in groups)
                    {
                        var stateType= (stateGroup.Value.GroupDefinition).GetType().GenericTypeArguments.FirstOrDefault();
                        var groupNotifier = inotifier.MakeGenericType(stateType);
                        if (stateGroup.Value.GetType().GetTypeInfo().ImplementedInterfaces.Contains(groupNotifier))
                        {
                            var fnt = typeof(FrameNavigation<>).MakeGenericType(stateType);
                            var fn = Activator.CreateInstance(fnt, frame, stateGroup.Value);
                        }
                    }
                }

#if WINDOWS_UWP
                frame.Navigated += Frame_Navigated;
#endif
                //var interfaces = region.GetType().GetInterfaces();
                //foreach (var it in interfaces)
                //{
                //    if (it.IsConstructedGenericType && 
                //    it.GetGenericTypeDefinition() == typeof(IHasViewModelStateManager<,>))
                //    {
                //        var args = it.GenericTypeArguments;
                //        var fnt = typeof (FrameNavigation<,>).MakeGenericType(args);
                //        var fn = Activator.CreateInstance(fnt, frame, region);//, string.Empty);
                //    }
                //}


                Window.Current.Content = frame;

                region.CloseRegion += Region_CloseRegion;

                if (hasStates != null)
                {
                    hasStates.StateManager.GoToPreviousStateIsBlockedChanged +=
                        StateManager_GoToPreviousStateIsBlockedChanged;
                }

                Window.Current.Activated += Current_Activated;

                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;

                RegionWindows[region.RegionId] = Window.Current.CoreWindow;

                if (isPrimary)
                {
                    region.Startup(sender as IRegionManager);
                }
                else { 
                Task.Run(async () => await region.Startup(sender as IRegionManager));
                }


            });

            if (!isPrimary)
            {
                var viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
                Debug.WriteLine(viewShown);
            }
#endif
        }

        private void StateManager_GoToPreviousStateIsBlockedChanged(object sender, EventArgs e)
        {
            UpdateAppViewBackButton();
        }

        private void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            
            var win = sender as Window;
//#if WINDOWS_PHONE_APP
//             if (e. != CoreWindowActivationState.Deactivated)
//                {
//                    Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
//                }
//                else
//                {
//                    Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
//                }
//# el
#if WINDOWS_UWP
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                if (e.WindowActivationState != CoreWindowActivationState.Deactivated)
                {
                    Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
                }
                else
                {
                    Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
                }
            }

            if (e.WindowActivationState != CoreWindowActivationState.Deactivated)
            {
                SubscribeToBackRequestedEvent();
            }
            else
            {
                UnsubscribeFromBackRequestedEvent();
            }
#endif
        }


#if WINDOWS_UWP
        private async void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            var backArgs = e;
            var wrapper = new ActionWrapper
            {
                WrappedAction = () => { backArgs.Handled = true; }
            };
            await GoToPreviousState(wrapper);
        }


        private void Frame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            UpdateAppViewBackButton();
            //var rootFrame = sender as Frame;
            //if (rootFrame != null)
            //{
            //    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
            //        rootFrame.CanGoBack
            //            ? AppViewBackButtonVisibility.Visible
            //            : AppViewBackButtonVisibility.Collapsed;
            //}

        }

#endif

        private void UpdateAppViewBackButton()
        {
            var hasStates = RegionForCurrentWindow as IHasStates;
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null)
            {
#if WINDOWS_UWP
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    (rootFrame.CanGoBack && !(hasStates?.StateManager.GoToPreviousStateIsBlocked ?? false))
                        ? AppViewBackButtonVisibility.Visible
                        : AppViewBackButtonVisibility.Collapsed;
#else
                // TODO: Find a Win8x equivalent
#endif
            }
        }

        public void SubscribeToBackRequestedEvent()
        {
#if WINDOWS_UWP
            SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
#endif
        }

        public void UnsubscribeFromBackRequestedEvent()
        {
#if WINDOWS_UWP
            SystemNavigationManager.GetForCurrentView().BackRequested -= BackRequested;
#endif
        }

#if WINDOWS_UWP
        protected async void BackRequested(object sender, BackRequestedEventArgs e)
        {
            var backArgs = e;
            var wrapper = new ActionWrapper
            {
                WrappedAction = () => { backArgs.Handled = true; }
            };
            await GoToPreviousState(wrapper);
        }
#endif
        private class ActionWrapper
        {
            public Action WrappedAction { get; set; }

            public void InvokeAction()
            {
                WrappedAction?.Invoke();
            }
        }

        private IApplicationRegion RegionForCurrentWindow
        {
            get
            {
                var win = Window.Current.CoreWindow;
                var regId = (from r in RegionWindows
                    where r.Value == win
                    select r.Key).FirstOrDefault();
                if (regId != null)
                {
                    return RegionManager.RegionById(regId);

                }
                return null;
            }
        }

        private async Task GoToPreviousState(ActionWrapper onsuccess)
        { 

        Debug.WriteLine("Going back");

            var reg = RegionForCurrentWindow as IHasStates;
            //var win = Window.Current.CoreWindow;
            //var regId = (from r in RegionWindows
            //    where r.Value == win
            //    select r.Key).FirstOrDefault();
            //if (regId != null)
            //{
            //    var reg = RegionManager.RegionById(regId) as IHasStates;
            if (reg?.StateManager.PreviousStateExists ?? false)
            {
                onsuccess.InvokeAction();
                if (!(reg?.StateManager.GoToPreviousStateIsBlocked ?? false))
                {
                    await reg.StateManager.GoBackToPreviousState();
                }
                return;
            }
            //}

            //var gb = GoBackViewModel;
            //if (gb != null)
            //{

            //    var cancel = new CancelEventArgs();
            //    await GoBackViewModel.GoingBack(cancel);
            //    if (cancel.Cancel)
            //    {
            //        e.Handled = true;
            //        return;
            //    }
            //}
            //e.Handled = true;
            //if (Frame.CanGoBack)
            //{
            //    Frame.GoBack();
            //}
        }


        private void Region_CloseRegion(object sender, EventArgs e)
        {
            RegionWindows[(sender as IApplicationRegion).RegionId].Close();
        }
    }
}