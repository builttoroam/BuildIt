using CognitiveServicesDemo.ViewModels;
using Octane.Xam.VideoPlayer.Constants;
using Octane.Xam.VideoPlayer.Events;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace CognitiveServicesDemo.Views
{
    public partial class VisionVideoFacialRecognitionPage : ContentPage
    {
        private static readonly TimeSpan HighlightTimerInterval = TimeSpan.FromSeconds(0.03);

        public VisionVideoFacialRecognitionViewModel CurrentViewModel
            => BindingContext as VisionVideoFacialRecognitionViewModel;

        private int highlightIndex;
        private TimeSpan lastDrawn = TimeSpan.Zero;

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
                    Name = $"{DateTime.Now:T}.mp4".Replace(":", "-")
                };
                //var file = await CrossMedia.Current.TakePhotoAsync(mediaOptions);
                var file = await CrossMedia.Current.TakeVideoAsync(mediaOptions);
                CurrentViewModel.VideoPath = file.Path;
                await CurrentViewModel.UploadVideoAsync(file);
                VideoPlayer.Source = CurrentViewModel.VideoPath;
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        private void DrawRectangles(object sender, EventArgs e)
        {
            //DrawRectangle(CurrentViewModel.Rectangles, VideoPlayer.Height, VideoPlayer.Width);
            //CurrentViewModel.VideoCurrentPosition = VideoPlayer.CurrentTime.Milliseconds;
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
                ResultRelativeLayout.Children.Add(frame,
                    Constraint.RelativeToView(VideoPlayer, (ResultRelativeLayout, DisplayImage) => (face.X * this.VideoPlayer.Width)),
                    Constraint.RelativeToView(VideoPlayer, (ResultRelativeLayout, DisplayImage) => (face.Y * VideoPlayer.Height)),
                    Constraint.RelativeToView(VideoPlayer, (ResultRelativeLayout, DisplayImage) => this.VideoPlayer.Width - (face.Width * VideoPlayer.Width)),
                    Constraint.RelativeToView(VideoPlayer, (ResultRelativeLayout, DisplayImage) => this.VideoPlayer.Height - (face.Height * VideoPlayer.Height)));
            }
        }

        private bool DrawRectangles()
        {
            var currentSecond = VideoPlayer.CurrentTime.TotalSeconds;
            var isPlaying = VideoPlayer.State == PlayerState.Playing;

            if (!isPlaying) return true;
            if (CurrentViewModel.Processing || CurrentViewModel.FrameHighlights == null || CurrentViewModel.FrameHighlights.Count <= 0) return true;

            while (highlightIndex < CurrentViewModel.FrameHighlights.Count &&
                   CurrentViewModel.FrameHighlights[highlightIndex].Time <= currentSecond)
            {
                highlightIndex++;
            }

            if (highlightIndex > CurrentViewModel.FrameHighlights.Count) return true;
            if (highlightIndex == 0) return true;

            var existingFrames = ResultRelativeLayout.Children.OfType<Frame>().ToList();
            foreach (var existingFrame in existingFrames)
            {
                ResultRelativeLayout.Children.Remove(existingFrame);
            }

            var frame = new Frame() { OutlineColor = Color.Red };

            var positions = CurrentViewModel.FrameHighlights[highlightIndex - 1].HighlightRects;
            for (int i = 0; i < positions.Length; i++)
            {
                var rect = positions[i];
                var w = VideoPlayer.Width;
                var h = VideoPlayer.Height;
                var vw = CurrentViewModel.NaturalVideoWidth;
                var vh = CurrentViewModel.NaturalVideoHeight;

                if (h > 0 && rect.Height > 0)
                {
                    var vr = vw / vh;
                    var offsetX = Math.Max((w - h * vr) / 2, 0);
                    var offsetY = Math.Max((h - w / vr) / 2, 0);

                    var realWidth = w - 2 * offsetX;
                    var realHeight = h - 2 * offsetY;

                    ResultRelativeLayout.Children.Add(frame,
                        Constraint.RelativeToView(VideoPlayer, (rl, v) => rect.X / vw * w),
                        Constraint.RelativeToView(VideoPlayer, (rl, v) => rect.Y / vh * h),
                        Constraint.RelativeToView(VideoPlayer, (rl, v) => rect.Width / vw * w),
                        Constraint.RelativeToView(VideoPlayer, (rl, v) => rect.Height / vh * h));

                }
            }

            return VideoPlayer.State == PlayerState.Playing;
        }

        private void VideoPlayer_OnPlaying(object sender, VideoPlayerEventArgs e)
        {
            DrawRectangles();
            Device.StartTimer(HighlightTimerInterval, DrawRectangles);
        }
    }
}
