using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
#if NETFX_CORE
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
#if !WIN8
using Cirrious.MvvmCross.WindowsCommon.Views;
#else
using Cirrious.MvvmCross.WindowsStore.Views;

#endif
#if WINDOWS_PHONE_APP || WINDOWS_UWP
using Windows.Phone.UI.Input;
#endif
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Cirrious.MvvmCross.WindowsPhone.Views;
#endif
using Cirrious.MvvmCross.ViewModels;
using System.Collections.Generic;
using Windows.UI.Core;
using BuildIt;
using BuildIt.MvvmCross.Interfaces;
using BuildIt.MvvmCross.ViewModels;

namespace BuiltToRoam.MvvmCross.UI.Views
{
    public class BaseStateEnabledPage :
#if NETFX_CORE && !WIN8
        MvxWindowsPage
#elif WINDOWS_PHONE
 MvxPhonePage
#else
        MvxStorePage
#endif
    {

        protected IStateAndTransitions StatesAndTransitionsViewModel
        {
            get
            {
                return DataContext as IStateAndTransitions;
            }
        }

        protected BuildIt.MvvmCross.Interfaces.ICanGoBack GoBackViewModel
        {
            get { return DataContext as ICanGoBack; }
        }

#if WINDOWS_UWP
        public static bool AutomaticallyShowAppViewBackButton { get; set; } = true;

        public virtual bool DisplayAppViewBackButton { get; } = true;

        public virtual bool CanSubscribeToBackRequest { get; } = true;
#endif

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                // Make sure we get a new VM each time we arrive at the page
                if (e.NavigationMode != NavigationMode.Back)
                {
                    DataContext = null;
                }

                base.OnNavigatedTo(e);

#if WINDOWS_PHONE_APP
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;

#elif WINDOWS_UWP
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
                }

                if (CanSubscribeToBackRequest)
                {
                    SubscribeToBackRequestedEvent();
                }
                if (AutomaticallyShowAppViewBackButton)
                {
                    if (DisplayAppViewBackButton)
                    {
                        var rootFrame = Window.Current.Content as Frame;
                        if (rootFrame != null)
                        {
                            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                                rootFrame.CanGoBack
                                    ? AppViewBackButtonVisibility.Visible
                                    : AppViewBackButtonVisibility.Collapsed;
                        }
                    }
                }
#endif
                if (GoBackViewModel != null)
                {
                    GoBackViewModel.ClearPreviousViews += ClearViews;
                }

                var satvm = StatesAndTransitionsViewModel;
                if (satvm != null)
                {
                    satvm.StateChanged += ViewModelStateChanged;
                    satvm.RunStoryboard += RunStoryboard;
                    satvm.StopStoryboard += StopStoryboard;

                    // Force a refresh of all current states
                    satvm.RefreshStates();
                }

                var bvm = ViewModel as BaseViewModel;
                if (bvm != null)

                {
                    await bvm.WaitForStartCompleted();
                }

                await OnNavigatedToCompleted();

            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }

#if WINDOWS_UWP
        public void SubscribeToBackRequestedEvent()
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
        }

        public void UnsubscribeFromBackRequestedEvent()
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= BackRequested;
        }

        protected virtual async void BackRequested(object sender, BackRequestedEventArgs e)
        {
            var gb = GoBackViewModel;
            if (gb != null)
            {

                var cancel = new CancelEventArgs();
                await GoBackViewModel.GoingBack(cancel);
                if (cancel.Cancel)
                {
                    e.Handled = true;
                    return;
                }
            }
            e.Handled = true;
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

#endif

        protected async virtual Task OnNavigatedToCompleted()
        {

        }

        private void ClearViews(object sender, EventArgs e)
        {
            try
            {
#if WINDOWS_PHONE_APP || NETFX_CORE
#if !WIN8
                if (Frame != null)
                {
                    Frame.BackStack.Clear();
                }
#endif
#else
                while (NavigationService.BackStack.Any())
                {
                    NavigationService.RemoveBackEntry();
                }
#endif 
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

#if WINDOWS_PHONE_APP || WINDOWS_UWP
        async void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            var gb = GoBackViewModel;
            if (gb != null)
            {

                var cancel = new CancelEventArgs();
                await GoBackViewModel.GoingBack(cancel);
                if (cancel.Cancel)
                {
                    e.Handled = true;
                    return;
                }
            }

            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame != null && rootFrame.CanGoBack)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

#endif

        private void ViewModelStateChanged(object sender, DualParameterEventArgs<string, bool> e)
        {
            try
            {
                //var controlName = e.Parameter1;
                var stateName = e.Parameter1;
                var useTransitions = e.Parameter2;

                // Locate the control to change state of (use this Page if controlNAme is null)
                Control control = this;
                // ReSharper disable AssignNullToNotNullAttribute
                VisualStateManager.GoToState(control, stateName, useTransitions);
                // ReSharper restore AssignNullToNotNullAttribute
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }

        }


        private void RunStoryboard(object sender, DualParameterEventArgs<string, Action> e)
        {
            try
            {
                var storyboardName = e.Parameter1;
                var completedAction = e.Parameter2;

                if (!string.IsNullOrEmpty(e.Parameter1))
                {
                    try
                    {

                        var sb = FindName(storyboardName) as Storyboard;
                        if (sb == null) return;
                        if (completedAction != null)
                        {
#if !NETFX_CORE
                            EventHandler eventHandler = null;
#else
                            EventHandler<object> eventHandler = null;
#endif
                            eventHandler = (s, es) =>
                            {
                                completedAction();
                                // ReSharper disable AccessToModifiedClosure
                                sb.Completed -= eventHandler;
                                // ReSharper restore AccessToModifiedClosure
                            };
                            sb.Completed += eventHandler;
                        }
                        sb.Begin();

                    }
                    // ReSharper disable EmptyGeneralCatchClause
                    catch
                    // ReSharper restore EmptyGeneralCatchClause
                    {
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }

        }

        /// <summary>
        /// Handles the stop storyboard event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopStoryboard(object sender, ParameterEventArgs<string> e)
        {
            try
            {
                var storyboardName = e.Parameter1;
                try
                {
                    if (!string.IsNullOrEmpty(storyboardName))
                    {
                        var sb = FindName(storyboardName) as Storyboard;
                        if (sb != null)
                        {
                            sb.Stop();
                        }
                    }
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch
                {
                }
                // ReSharper restore EmptyGeneralCatchClause
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            try
            {
#if WINDOWS_PHONE_APP
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;

#elif WINDOWS_UWP
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
                }
                if (CanSubscribeToBackRequest)
                {
                    UnsubscribeFromBackRequestedEvent();
                }
#endif
                if (GoBackViewModel != null)
                {
                    GoBackViewModel.ClearPreviousViews -= ClearViews;
                }

                var satvm = StatesAndTransitionsViewModel;
                if (satvm != null)
                {
                    satvm.StateChanged -= ViewModelStateChanged;
                    satvm.RunStoryboard -= RunStoryboard;
                    satvm.StopStoryboard -= StopStoryboard;
                    //EventsWired = false;
                }

                base.OnNavigatedFrom(e);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }

        }

#if !NETFX_CORE
        protected override void SaveStateBundle(NavigationEventArgs navigationEventArgs, IMvxBundle bundle)
        {
            try
            {
                base.SaveStateBundle(navigationEventArgs, bundle);

                foreach (var data in bundle.Data)
                {
                    State[data.Key] = data.Value;
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }

        }

        protected override IMvxBundle LoadStateBundle(NavigationEventArgs navigationEventArgs)
        {
            try
            {
                var bundle = base.LoadStateBundle(navigationEventArgs);
                if (bundle == null)
                {
                    bundle = new MvxBundle();
                }
                foreach (var data in State)
                {
                    bundle.Data[data.Key] = data.Value.ToString();
                }
                return bundle;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return null;
            }
        }
#endif
    }
}
