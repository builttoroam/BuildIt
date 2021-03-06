﻿using BuildIt.Forms.Animations;
using BuildIt.Forms.Controls;
using BuildIt.Forms.Sample.Core.ViewModels;
using BuildIt.States.Interfaces;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

            CameraPreviewControl.RequestCameraPermissionsCallback = RequestCameraPermissionsCallback;
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

        private async void ShowTab5Clicked(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "ShowTab5");
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

        private async void StartCameraButton_OnClicked(object sender, EventArgs e)
        {
            await CameraPreviewControl.StartPreview();
        }

        private async void StopCameraButton_OnClicked(object sender, EventArgs e)
        {
            await CameraPreviewControl.StopPreview();
        }

        private void FlipCameraButton_OnClicked(object sender, EventArgs e)
        {
            CameraPreviewControl.PreferredCamera =
                CameraPreviewControl.PreferredCamera == CameraFacing.Back
                    ? CameraFacing.Front
                    : CameraFacing.Back;
        }

        private async void PhotoButton_OnClicked(object sender, EventArgs e)
        {
            await TakePhoto();
        }

        private async void FocusAndTakePhoto_OnClicked(object sender, EventArgs e)
        {
            var focusSucceeded = await CameraPreviewControl.TryFocusing();
            if (focusSucceeded)
            {
                await TakePhoto();
            }
        }

        private void RetrieveSupportedFocusModesButton_Clicked(object sender, EventArgs e)
        {
            var supportedFocusModes = CameraPreviewControl.RetrieveSupportedFocusModes();
            SupportedFocusModesLabel.Text = string.Join(", ", supportedFocusModes);
        }

        private async void RetrieveAvailableCameras_Clicked(object sender, EventArgs e)
        {
            var cameras = await CameraPreviewControl.RetrieveCamerasAsync();
            AvailableCamerasLabel.Text = string.Join(Environment.NewLine, cameras.Select(c => $"Camera id: {c.Id}, facing: {c.CameraFacing}"));
        }

        private void PreviewAspectFitButton_Clicked(object sender, EventArgs e)
        {
            CameraPreviewControl.Aspect = Aspect.AspectFit;
        }

        private void PreviewAspectFillButton_Clicked(object sender, EventArgs e)
        {
            CameraPreviewControl.Aspect = Aspect.AspectFill;
        }

        private void PreviewFillButton_Clicked(object sender, EventArgs e)
        {
            CameraPreviewControl.Aspect = Aspect.Fill;
        }

        private async Task<bool> RequestCameraPermissionsCallback()
        {
            if (await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera) == PermissionStatus.Granted)
            {
                return true;
            }

            // need to request runtime permissions for using the camera
            var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
            if (results.ContainsKey(Permission.Camera))
            {
                var status = results[Permission.Camera];
                return status == PermissionStatus.Granted;
            }

            return false;
        }

        private void AutoFocusMode_OnClicked(object sender, EventArgs e)
        {
            SetFocusMode(CameraFocusMode.Auto);
        }

        private void ContinueFocusMode_OnClicked(object sender, EventArgs e)
        {
            SetFocusMode(CameraFocusMode.Continuous);
        }

        private void ManualFocusMode_OnClicked(object sender, EventArgs e)
        {
            SetFocusMode(CameraFocusMode.Manual);
        }

        private async void TryFocusing_OnClicked(object sender, EventArgs e)
        {
            var currentViewModel = BindingContext as MainViewModel;
            if (currentViewModel == null || currentViewModel.CameraFocusMode != CameraFocusMode.Auto)
            {
                return;
            }

            await CameraPreviewControl.TryFocusing();
        }

        private async Task TakePhoto()
        {
            var path = await CameraPreviewControl.CaptureFrameToFile(true);
            var result = await DisplayAlert("Photo Captured", path, "View Photo", "Cancel");
            if (result)
            {
                await Navigation.PushModalAsync(new SavedPhotoPage(path), true);
            }
        }

        private void SetFocusMode(CameraFocusMode focusMode)
        {
            var currentViewModel = BindingContext as MainViewModel;
            if (currentViewModel == null)
            {
                return;
            }

            currentViewModel.CameraFocusMode = focusMode;
        }

        private async void ChangeStatefulControlToEmpty_OnClicked(object sender, EventArgs e)
        {
            await (BindingContext as MainViewModel).UpdateStatefulControlState(StatefulControlStates.Empty);
        }

        private async void ChangeStatefulControlToLoadingError_OnClicked(object sender, EventArgs e)
        {
            await (BindingContext as MainViewModel).UpdateStatefulControlState(StatefulControlStates.LoadingError);
        }

        private async void ChangeStatefulControlToLoaded_OnClicked(object sender, EventArgs e)
        {
            await (BindingContext as MainViewModel).UpdateStatefulControlState(StatefulControlStates.Loaded);
        }
    }
}