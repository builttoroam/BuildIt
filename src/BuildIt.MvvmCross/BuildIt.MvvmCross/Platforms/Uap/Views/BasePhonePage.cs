using BuildIt.MvvmCross.Interfaces;
using BuildIt.MvvmCross.ViewModels;
using MvvmCross.Platforms.Uap.Views;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace BuildIt.MvvmCross.Views
{
    public class BaseStateEnabledPage : MvxWindowsPage
    {
        protected IStateAndStoryboards StatesAndTransitionsViewModel
        {
            get
            {
                return DataContext as IStateAndStoryboards;
            }
        }

        protected BuildIt.MvvmCross.Interfaces.ICanGoBack GoBackViewModel
        {
            get { return DataContext as ICanGoBack; }
        }

        public static bool AutomaticallyShowAppViewBackButton { get; set; } = true;

        public static bool EnablePageCaching { get; set; } = false;

        public virtual bool DisplayAppViewBackButton { get; } = true;

        public virtual bool CanSubscribeToBackRequest { get; } = true;

        public BaseStateEnabledPage()
        {
            if (EnablePageCaching)
            {
                NavigationCacheMode = NavigationCacheMode.Required;
            }
        }

        protected virtual void DumpExistingViewModel(NavigationEventArgs e)
        {
            // Make sure we get a new VM each time we arrive at the page
            if (e.NavigationMode != NavigationMode.Back)
            {
                DataContext = null;
                ViewModel = null;
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                DumpExistingViewModel(e);

                base.OnNavigatedTo(e);

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

                if (GoBackViewModel != null)
                {
                    GoBackViewModel.ClearPreviousViews += ClearViews;
                }

                var satvm = StatesAndTransitionsViewModel;
                if (satvm != null)
                {
                    satvm.StateManager.StateChanged += ViewModelStateChanged;
                    satvm.RunStoryboard += RunStoryboard;
                    satvm.StopStoryboard += StopStoryboard;

                    // Force a refresh of all current states
                    satvm.StateManager.RefreshStates();
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

#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected virtual async Task OnNavigatedToCompleted()
#pragma warning restore 1998
        {
        }

        private void ClearViews(object sender, EventArgs e)
        {
            try
            {
                if (Frame != null)
                {
                    Frame.BackStack.Clear();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void ViewModelStateChanged(object sender, DualParameterEventArgs<string, bool> e)
        {
            try
            {
                // var controlName = e.Parameter1;
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
                        if (sb == null)
                        {
                            return;
                        }

                        if (completedAction != null)
                        {
                            EventHandler<object> eventHandler = null;
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
        /// Handles the stop storyboard event.
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
                if (CanSubscribeToBackRequest)
                {
                    UnsubscribeFromBackRequestedEvent();
                }

                if (GoBackViewModel != null)
                {
                    GoBackViewModel.ClearPreviousViews -= ClearViews;
                }

                var satvm = StatesAndTransitionsViewModel;
                if (satvm != null)
                {
                    satvm.StateManager.StateChanged -= ViewModelStateChanged;
                    satvm.RunStoryboard -= RunStoryboard;
                    satvm.StopStoryboard -= StopStoryboard;
                    // EventsWired = false;
                }

                base.OnNavigatedFrom(e);
                if (EnablePageCaching)
                {
                    if (e.NavigationMode == NavigationMode.Back)
                    {
                        NavigationCacheMode = NavigationCacheMode.Disabled;
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }
    }
}