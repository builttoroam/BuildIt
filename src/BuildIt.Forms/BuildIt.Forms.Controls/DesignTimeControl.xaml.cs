using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildIt.States.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BuildIt.Forms.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DesignTimeControl : ContentView
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
    }

    public class DesignInfo : NotifyBase
    {
        private IStateGroup[] groups;
        private IStateGroup selectedGroup;
        private IStateDefinition[] states;

        public DesignInfo(Element element)
        {
            Element = element;
            var sm = VisualStateManager.GetVisualStateGroups(element);
            StateManager = sm.StateManager;
        }

        public Element Element { get; set; }

        public IStateGroup[] Groups => groups ?? (groups = StateManager.StateGroups.Select(x => x.Value).ToArray());

        public IStateDefinition[] States
        {
            get { return states; }
            set
            {
                states = value;
                OnPropertyChanged();
            }
        }

        public IStateGroup SelectedGroup
        {
            get { return selectedGroup; }
            set
            {
                selectedGroup = value;
                if (selectedGroup == null)
                {
                    return;
                }

                States = selectedGroup.GroupDefinition.States.Select(x => x.Value).ToArray();
            }
        }


        private IStateManager StateManager { get; }

    }

    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }
            // Do your translation lookup here, using whatever method you require
            var imageSource = ImageSource.FromResource(Source);

            return imageSource;
        }
    }
}