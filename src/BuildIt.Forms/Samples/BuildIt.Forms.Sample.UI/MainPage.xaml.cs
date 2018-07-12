using BuildIt.Forms.Sample.Core.ViewModels;
using BuildIt.Forms.Animations;
using BuildIt.States.Interfaces;
using System;
using System.Threading;
using BuildIt.Forms.Controls;
using Xamarin.Forms;

namespace BuildIt.Forms.Sample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            BindingContext = new MainViewModel();

            VisualStateManager.Bind(this, (BindingContext as IHasStates)?.StateManager);

            //  DesignTimeInfo.BindingContext = new DesignInfo(this);

            this.AddDesignAction("Test", async () =>
            {
                HelloWorldText.Text = "Hi Everyone!";
                "Hi changed".LogMessage();
                await VisualStateManager.GoToState(this, "RotateRight");
                "Hi changed back".LogMessage();
                HelloWorldText.Text = "Well, maybe not everyone";
                await VisualStateManager.GoToState(this, "RotateLeft");
                "Ending".LogMessage();
            });
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

        public async void RotateLeftClicked(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "RotateLeft");

            var sb = Resources["FadeToHalf"] as Storyboard;
            await sb.Animate(CancellationToken.None);
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

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await Custom.BindVisualStates();

            await (BindingContext as MainViewModel).Init();
        }

        protected async void OnTouchEffectAction(object sender, object args)
        {
            "Touched - mainpage".LogMessage();
        }

        private void ContentButton_OnPressed(object sender, EventArgs e)
        {
            "IconButton Pressed - MainPage".LogMessage();
        }

        private async void ShowTab1Clicked(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "ShowTab1");
        }
        private async void ShowTab2Clicked(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "ShowTab2");
        }
        private async void ShowTab3Clicked(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "ShowTab3");
        }
        private async void ShowTab4Clicked(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "ShowTab4");
        }

        private void BobClick(object sender, EventArgs e)
        {
            (BindingContext as MainViewModel).LoadBob();
        }
        private void FredClick(object sender, EventArgs e)
        {
            (BindingContext as MainViewModel).LoadFred();

        }

        private void Bob2Click(object sender, EventArgs e)
        {
            (BindingContext as MainViewModel).LoadBob2();
        }

        private void MutateClick(object sender, EventArgs e)
        {
            (BindingContext as MainViewModel).Mutate();

        }

        private void FlipCameraButton_OnClicked(object sender, EventArgs e)
        {
            CameraPreviewControl.PreferredCamera =
                CameraPreviewControl.PreferredCamera == CameraPreviewControl.CameraPreference.Back
                    ? CameraPreviewControl.CameraPreference.Front
                    : CameraPreviewControl.CameraPreference.Back;
        }
    }
}