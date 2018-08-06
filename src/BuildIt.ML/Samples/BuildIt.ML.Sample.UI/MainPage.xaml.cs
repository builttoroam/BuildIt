using BuildIt.ML.Sample.Core;
using System;

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
        }

        private async void ClassifyButton_Clicked(object sender, EventArgs e)
        {
            CameraPreviewControl.IsVisible = true;
        }

        private async void CameraPreviewControl_MediaFrameArrived(object sender, Forms.Controls.MediaFrameEventArgs eventArgs)
        {
            await ViewModel?.ClassifyAsync(eventArgs.MediaFrame.NativeFrame);
        }
    }
}