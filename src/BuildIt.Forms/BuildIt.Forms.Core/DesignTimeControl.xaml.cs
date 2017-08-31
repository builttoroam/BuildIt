using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BuildIt.States.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BuildIt.Forms
{
    /// <summary>
    /// A control that allows developers to force visual state changes
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DesignTimeControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DesignTimeControl"/> class.
        /// </summary>
        public DesignTimeControl()
        {
            // Only show the design time control if the debugger is attached
            if (!Debugger.IsAttached)
            {
                return;
            }

            InitializeComponent();
        }

        private enum DesignStates
        {
            Base,
            GroupsHidden,
            GroupsVisible
        }

        /// <summary>
        /// Handles the selection changed event for design actions
        /// </summary>
        /// <param name="sender">The listview containing design actions</param>
        /// <param name="e">The selection change</param>
        public async void DesignActionSelectionChanged(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                await HideGroups();

                var action = e.SelectedItem as Tuple<string, Action>;
                $"Running design action {action?.Item1}".Log();
                action?.Item2();

                if (sender is ListView lv)
                {
                    lv.SelectedItem = null;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        /// <summary>
        /// Handles the selection changed event for groups
        /// </summary>
        /// <param name="sender">The listview containing groups</param>
        /// <param name="e">The selection change</param>
        public void GroupSelectionChanged(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                "StateGroupList selection changed".Log();

                if (e.SelectedItem == null)
                {
                    "StateGroupList no selection".Log();
                    States.IsVisible = false;
                    return;
                }

                var design = (sender as Element)?.BindingContext as DesignInfo;
                var group = e.SelectedItem as IStateGroup;
                if (design == null || group == null)
                {
                    "no context or group".Log();
                    return;
                }

                ("Group: " + group.GroupName).Log();
                design.SelectedGroup = group;
                States.IsVisible = true;

                "SateGroupList selection changed - END".Log();
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        /// <summary>
        /// The selection changed handler for states
        /// </summary>
        /// <param name="sender">The listview of states</param>
        /// <param name="e">The selection change</param>
        public async void StateSelectionChanged(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                "StateList selection changed".Log();

                if (e.SelectedItem == null)
                {
                    "StateList no selection".Log();
                    return;
                }

                var design = (sender as Element)?.BindingContext as DesignInfo;
                var state = e.SelectedItem as IStateDefinition;

                StatesList.SelectedItem = null;

                StateGroupList.SelectedItem = null;
                await HideGroups();

                if (design == null || state == null)
                {
                    "No context or state".Log();
                    return;
                }

                ("State: " + state.StateName).Log();
                await VisualStateManager.GoToState(design.Element, state.StateName);
                "SateList selection changed - END".Log();
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        /// <summary>
        /// Launches the state picker
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The event args</param>
        protected async void LaunchDesignTime(object sender, object args)
        {
            try
            {
                var touchArgs = args as TouchActionEventArgs;
                if (touchArgs == null ||
                    (touchArgs.Type != TouchActionType.Pressed && touchArgs.Type != TouchActionType.Released))
                {
                    return;
                }

                "Launching".Log();
                await ShowGroups();
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private async Task HideGroups()
        {
            await VisualStateManager.GoToState(this, DesignStates.GroupsHidden.ToString());
            this.HorizontalOptions = LayoutOptions.Start;
            this.VerticalOptions = LayoutOptions.End;
        }

        private async Task ShowGroups()
        {
            this.HorizontalOptions = LayoutOptions.Fill;
            this.VerticalOptions = LayoutOptions.Fill;
            await VisualStateManager.GoToState(this, DesignStates.GroupsVisible.ToString());
        }

        /// <summary>
        /// Launches the Built to Roam website
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The event args</param>
        protected void LaunchBuiltToRoamWebsite(object sender, object args)
        {
            try
            {
                var touchArgs = args as TouchActionEventArgs;
                if (touchArgs == null ||
                    (touchArgs.Type != TouchActionType.Pressed && touchArgs.Type != TouchActionType.Released))
                {
                    return;
                }

                Device.OpenUri(new Uri("https://www.builttoroam.com"));
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        /// <summary>
        /// Exits the state picker
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The event args</param>
        protected async void ExitDesignTime(object sender, object args)
        {
            try
            {
                var touchArgs = args as TouchActionEventArgs;
                if (touchArgs == null ||
                    (touchArgs.Type != TouchActionType.Pressed && touchArgs.Type != TouchActionType.Released))
                {
                    return;
                }

                StatesList.SelectedItem = null;

                StateGroupList.SelectedItem = null;
                await HideGroups();
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }
}