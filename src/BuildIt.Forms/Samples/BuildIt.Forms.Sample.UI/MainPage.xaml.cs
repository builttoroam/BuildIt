using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildIt.forms.Sample.Core.ViewModels;
using Xamarin.Forms;
using BuildIt.Forms.Controls;
using BuildIt.ServiceLocation;
using BuildIt.States;
using BuildIt.States.Interfaces;

namespace BuildIt.Forms.Sample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            BindingContext = new MainViewModel();

            // var x = new Label();
            // x.TextColor

            // var x = new Button();
            // x.TextColor

            // var x = new Frame();
            // x.BackgroundColor;

            // var x = new BoxView();
            // x.Color;

            // var cv = new ContentView();
            // cv.BackgroundColor;

            VisualStateManager.Bind(this, (BindingContext as IHasStates)?.StateManager);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await Custom.BindVisualStates();

            await (BindingContext as MainViewModel).Init();
        }

        public void ToggleButtonPressed(object sender, EventArgs e)
        {
            (BindingContext as MainViewModel).SwitchStates();
            // VisualStateManager.GoToState(this, visible ? "Show":"Hide");
        }

        public void ExpandClicked(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "DoubleHeight");
        }


        public void RotateLeftClicked(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "RotateLeft");
        }
        public void RotateRightClicked(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "RotateRight");
        }

        public void DefaultClicked(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "Default");
        }

        public void ButtonClicked(object sender, EventArgs e)
        {
            DisabledButton.IsEnabled = !DisabledButton.IsEnabled;
            (BindingContext as MainViewModel).CommandIsEnabled = DisabledButton.IsEnabled;
        }
    }
}
