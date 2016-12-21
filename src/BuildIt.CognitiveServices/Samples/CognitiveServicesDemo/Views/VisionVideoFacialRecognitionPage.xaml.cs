﻿using CognitiveServicesDemo.ViewModels;
using Octane.Xam.VideoPlayer.Events;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Diagnostics;
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
        private bool videoPlaying;

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

                var mediaOptions = new StoreVideoOptions
                {
                    Directory = "Receipts",
                    Name = $"{DateTime.Now:T}.mp4".Replace(":", "-")
                };
                //var file = await CrossMedia.Current.TakePhotoAsync(mediaOptions);
                var file = await CrossMedia.Current.TakeVideoAsync(mediaOptions);
                CurrentViewModel.VideoPath = file.Path;
                await CurrentViewModel.UploadVideoAsync(file);
                VideoPlayer.Source = CurrentViewModel.VideoPath;
                DrawRectangles();
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        private bool DrawRectangles()
        {
            var currentSecond = VideoPlayer.CurrentTime.TotalSeconds;
            //var isPlaying = VideoPlayer.State == PlayerState.Playing;

            //if (!isPlaying) return true;
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

            var positions = CurrentViewModel.FrameHighlights[highlightIndex - 1].HighlightRects;
            for (int i = 0; i < positions.Length; i++)
            {
                var rect = positions[i];
                var w = VideoPlayer.Width;
                var h = VideoPlayer.Height;
                //var vw = CurrentViewModel.NaturalVideoWidth;
                //var vh = CurrentViewModel.NaturalVideoHeight;

                if (h > 0 && rect.Height > 0)
                {
                    //var vr = vw / vh;
                    //var offsetX = Math.Max((w - h * vr) / 2, 0);
                    //var offsetY = Math.Max((h - w / vr) / 2, 0);

                    //var realWidth = w - 2 * offsetX;
                    //var realHeight = h - 2 * offsetY;

                    var rectX = rect.X * w;
                    var rectY = rect.Y * h;
                    var rectW = rect.Width * w;
                    var rectH = rect.Height * h;

                    var frame = new Frame { OutlineColor = Color.Red };
                    Debug.WriteLine($"Adding frame with values: X={rectX} Y={rectY} Width={rectW} Height={rectH}");

                    ResultRelativeLayout.Children.Add(frame,
                        Constraint.RelativeToView(VideoPlayer, (rl, v) => rectX),
                        Constraint.RelativeToView(VideoPlayer, (rl, v) => rectY),
                        Constraint.RelativeToView(VideoPlayer, (rl, v) => rectW),
                        Constraint.RelativeToView(VideoPlayer, (rl, v) => rectH));

                }
            }

            return videoPlaying;
        }

        private void VideoPlayer_OnPlaying(object sender, VideoPlayerEventArgs e)
        {
            videoPlaying = true;
            Device.StartTimer(HighlightTimerInterval, DrawRectangles);
        }

        private void VideoPlayer_OnCompleted(object sender, VideoPlayerEventArgs e)
        {
            videoPlaying = false;
        }
    }
}
