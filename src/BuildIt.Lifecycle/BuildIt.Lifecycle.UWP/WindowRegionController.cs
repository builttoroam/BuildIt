using BuildIt.Lifecycle.Interfaces;
using BuildIt.States.Interfaces;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BuildIt.Lifecycle
{
    public class WindowRegionController
    {
        private Window Window { get; }

        private IRegionManager RegionManager { get; }

        private IApplicationRegion Region { get; }

        private bool IsPrimaryRegion { get; }

        public WindowRegionController(
            Window window,
            IRegionManager regionManager,
            IApplicationRegion region,
            bool isPrimary)
        {
            Window = window;
            RegionManager = regionManager;
            Region = region;
            IsPrimaryRegion = isPrimary;
        }

        public int Start()
        {
            var frame = new Frame();

            var region = Region;

            var hasStates = region as IHasStates;
            var sm = (region as IHasStates)?.StateManager;
            if (sm != null)
            {

                var groups = sm.StateGroups;
                var inotifier = typeof(INotifyTypedStateChanged<>);
                foreach (var stateGroup in groups)
                {
                    var stateType =
                        stateGroup.Value.GroupDefinition.GetType().GenericTypeArguments.FirstOrDefault();
                    var groupNotifier = inotifier.MakeGenericType(stateType);
                    if (stateGroup.Value.GetType().GetTypeInfo().ImplementedInterfaces.Contains(groupNotifier))
                    {
                        var fnt = typeof(FrameNavigation<>).MakeGenericType(stateType);
                        var fn = Activator.CreateInstance(fnt, frame, stateGroup.Value);
                    }
                }
            }

            frame.Navigated += Frame_Navigated;

            Window.Content = frame;

            region.CloseRegion += Region_CloseRegion;

            if (hasStates != null)
            {
                hasStates.StateManager.GoToPreviousStateIsBlockedChanged +=
                    StateManager_GoToPreviousStateIsBlockedChanged;
            }

            Window.Activated += Current_Activated;

            Window.Activate();

            var newViewId = ApplicationView.GetForCurrentView().Id;

            if (IsPrimaryRegion)
            {
                region.Startup(RegionManager);
            }
            else
            {
                Task.Run(async () => await region.Startup(RegionManager));
            }

            return newViewId;
        }

        public void SubscribeToBackRequestedEvent()
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
        }

        public void UnsubscribeFromBackRequestedEvent()
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= BackRequested;
        }

        public void Close()
        {
            Window.CoreWindow.Close();
        }

        private void StateManager_GoToPreviousStateIsBlockedChanged(object sender, EventArgs e)
        {
            UpdateAppViewBackButton();
        }

        private void Current_Activated(object sender, WindowActivatedEventArgs e)
        {

            var win = sender as Window;
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
        }

        private async void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            var backArgs = e;
            var wrapper = new Action(() => { backArgs.Handled = true; });
            await GoToPreviousState(wrapper);
        }


        private void Frame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            UpdateAppViewBackButton();
        }

        private void UpdateAppViewBackButton()
        {
            var hasStates = Region as IHasStates;
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    (rootFrame.CanGoBack && !(hasStates?.StateManager.GoToPreviousStateIsBlocked ?? false))
                        ? AppViewBackButtonVisibility.Visible
                        : AppViewBackButtonVisibility.Collapsed;
            }
        }



        protected async void BackRequested(object sender, BackRequestedEventArgs e)
        {
            var backArgs = e;
            var wrapper = new Action(() => { backArgs.Handled = true; });
            await GoToPreviousState(wrapper);
        }

        private async Task GoToPreviousState(Action onsuccess)
        {

            Debug.WriteLine("Going back");

            var reg = Region as IHasStates;
            if (reg?.StateManager.PreviousStateExists ?? false)
            {
                onsuccess();
                if (!(reg?.StateManager.GoToPreviousStateIsBlocked ?? false))
                {
                    await reg.StateManager.GoBackToPreviousState();
                }
            }
        }

        private void Region_CloseRegion(object sender, EventArgs e)
        {
            Close();
        }
    }
}