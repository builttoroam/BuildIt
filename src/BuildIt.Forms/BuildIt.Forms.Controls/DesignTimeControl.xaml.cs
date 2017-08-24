using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using BuildIt.States.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BuildIt.Forms.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DesignTimeControl
    {
        public DesignTimeControl()
        {
            // Only show the design time control if the debugger is attached
            if (!Debugger.IsAttached)
            {
                return;
            }

            InitializeComponent();
        }

        private void DesignTimeControl_BindingContextChanged(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public void GroupSelectionChanged(object sender, SelectedItemChangedEventArgs e)
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

        public async void StateSelectionChanged(object sender, SelectedItemChangedEventArgs e)
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
            await VisualStateManager.GoToState(this, "GroupsHidden");

            if (design == null || state == null)
            {
                "No context or state".Log();
                return;
            }

            ("State: " + state.StateName).Log();
            await VisualStateManager.GoToState(design.Element, state.StateName);
            "SateList selection changed - END".Log();
        }

        protected async void LaunchDesignTime(object sender, object args)
        {
            var touchArgs = args as TouchActionEventArgs;
            if (touchArgs == null ||
                touchArgs.Type != TouchActionType.Pressed)
            {
                return;
            }

            "Launching".Log();
            await VisualStateManager.GoToState(this, "GroupsVisible");
        }

        protected async void ExitDesignTime(object sender, object args)
        {
            var touchArgs = args as TouchActionEventArgs;
            if (touchArgs == null ||
                touchArgs.Type != TouchActionType.Pressed)
            {
                return;
            }

            StatesList.SelectedItem = null;

            StateGroupList.SelectedItem = null;
            await VisualStateManager.GoToState(this, "GroupsHidden");
        }
    }
}