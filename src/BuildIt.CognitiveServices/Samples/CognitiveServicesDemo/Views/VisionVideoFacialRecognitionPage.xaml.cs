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

        private void DrawRectangles(object sender, EventArgs e)
        {
            DrawRectangle(CurrentViewModel.Rectangles, VideoPlayer.Height, VideoPlayer.Width);
            CurrentViewModel.VideoCurrentPosition = VideoPlayer.CurrentTime.Milliseconds;
        }

        public void DrawRectangle(List<Rectangle> rectangle, double imageWidth, double imageHeight)
        {
            //DisplayImage.Source = imageUri;
            var height = VideoPlayer.Height;
            var width = VideoPlayer.Width;
            var existingFrames = ResultRelativeLayout.Children.OfType<Frame>().ToList();
            foreach (var existingFrame in existingFrames)
            {
                ResultRelativeLayout.Children.Remove(existingFrame);
            }
            var frame = new Frame() { OutlineColor = Color.Red };


            //ResultRelativeLayout.Children.Add(frame, Constraint.RelativeToView(VideoPlayer, (ResultRelativeLayout, DisplayImage) => (rectangle[1].X * this.VideoPlayer.Width)), Constraint.RelativeToView(VideoPlayer, (ResultRelativeLayout, DisplayImage) => (rectangle[1].Y * VideoPlayer.Height)), Constraint.RelativeToView(VideoPlayer, (ResultRelativeLayout, DisplayImage) => this.VideoPlayer.Width - (rectangle[1].Width * VideoPlayer.Width)), Constraint.RelativeToView(VideoPlayer, (ResultRelativeLayout, DisplayImage) => this.VideoPlayer.Height - (rectangle[1].Height * VideoPlayer.Height)));
            foreach (var face in rectangle)
            {
                ResultRelativeLayout.Children.Add(frame, Constraint.RelativeToView(VideoPlayer, (ResultRelativeLayout, DisplayImage) => (face.X * this.VideoPlayer.Width)), Constraint.RelativeToView(VideoPlayer, (ResultRelativeLayout, DisplayImage) => (face.Y * VideoPlayer.Height)), Constraint.RelativeToView(VideoPlayer, (ResultRelativeLayout, DisplayImage) => this.VideoPlayer.Width - (face.Width * VideoPlayer.Width)), Constraint.RelativeToView(VideoPlayer, (ResultRelativeLayout, DisplayImage) => this.VideoPlayer.Height - (face.Height * VideoPlayer.Height)));
            }
        }
    }
}
