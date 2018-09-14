using BuildIt.Forms.Controls;
using BuildIt.ML.Sample.Core;
using System;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BuildIt.ML.Sample.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        private MainViewModel ViewModel => BindingContext as MainViewModel;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await ViewModel?.InitAsync();
            CameraPreviewControl.MediaFrameArrived = ClassifyImage;
            CameraPreviewControl.RequestCameraPermissionsCallback = RequestCameraPermissionsCallback;
        }

        private async Task ClassifyImage(MediaFrame mediaFrame)
        {
            await ViewModel?.ClassifyAsync(mediaFrame.NativeFrame);
        }

        private async void ClassifyButton_Clicked(object sender, EventArgs e)
        {
            await CameraPreviewControl.StartPreview();
        }

        private async void StopClassifyButton_Clicked(object sender, EventArgs e)
        {
            await CameraPreviewControl.StopPreview();
        }

        private async void CameraPreviewControl_MediaFrameArrived(object sender, Forms.Controls.MediaFrameEventArgs eventArgs)
        {
            await ViewModel?.ClassifyAsync(eventArgs.MediaFrame.NativeFrame);
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
    }
}