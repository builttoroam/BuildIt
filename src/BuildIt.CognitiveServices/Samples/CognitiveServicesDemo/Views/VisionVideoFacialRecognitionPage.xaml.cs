using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CognitiveServicesDemo.ViewModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace CognitiveServicesDemo.Views
{
    public partial class VisionVideoFacialRecognitionPage : ContentPage
    {
        public VisionVideoFacialRecognitionViewModel CurrentViewModel
            => BindingContext as VisionVideoFacialRecognitionViewModel;

        public VisionVideoFacialRecognitionPage()
        {
            InitializeComponent();
        }

        private async void TakeVideo_OnClicked(object sender, EventArgs e)
        {
            try
            {
                await CrossMedia.Current.Initialize();
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported) return;
                
                var mediaOptions = new StoreVideoOptions()
                {
                    Directory = "Receipts",
                    Name = $"{DateTime.UtcNow}.jpg"
                };
                //var file = await CrossMedia.Current.TakePhotoAsync(mediaOptions);
                var file = await CrossMedia.Current.TakeVideoAsync(mediaOptions);
                CurrentViewModel.VideoPath = file.Path;
                CurrentViewModel.UploadVideoAsync(file);
                VideoPlayer.Source = CurrentViewModel.VideoPath;
            }
            catch (Exception ex)
            {
                // ignored
            }
        }
    }
}
