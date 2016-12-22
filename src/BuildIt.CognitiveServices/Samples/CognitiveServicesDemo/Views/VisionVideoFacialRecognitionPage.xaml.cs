using CognitiveServicesDemo.ViewModels;
using Octane.Xam.VideoPlayer.Events;
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

        private bool DrawRectangles()
        {
            var existingFrames = ResultRelativeLayout.Children.OfType<Frame>().ToList();
            foreach (var existingFrame in existingFrames)
            {
                ResultRelativeLayout.Children.Remove(existingFrame);
            }

            var currentSecond = VideoPlayer.CurrentTime.TotalSeconds;

            if (CurrentViewModel.Processing || CurrentViewModel.FrameHighlights == null || CurrentViewModel.FrameHighlights.Count <= 0) return true;

            while (highlightIndex < CurrentViewModel.FrameHighlights.Count &&
                   CurrentViewModel.FrameHighlights[highlightIndex].Time <= currentSecond)
            {
                highlightIndex++;
            }

            if (highlightIndex > CurrentViewModel.FrameHighlights.Count) return true;
            if (highlightIndex <= 0) return true;


            var w = VideoPlayer.Width;
            var h = VideoPlayer.Height;
            var vw = CurrentViewModel.NaturalVideoWidth;
            var vh = CurrentViewModel.NaturalVideoHeight;
            var vr = vw / vh;
            var offsetX = Math.Max((w - h * vr) / 2, 0);
            var offsetY = Math.Max((h - w / vr) / 2, 0);

            var realWidth = w - 2 * offsetX;
            var realHeight = h - 2 * offsetY;

            var positions = CurrentViewModel.FrameHighlights[highlightIndex - 1].HighlightRects;
            for (int i = 0; i < positions.Length; i++)
            {
                var rect = positions[i];

                if (h > 0 && rect.Height > 0)
                {
                    var rectX = offsetX + rect.X * realWidth;
                    var rectY = offsetY + rect.Y * realHeight;
                    var rectW = rect.Width * realWidth;
                    var rectH = rect.Height * realHeight;

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
            highlightIndex = 0;
            Device.StartTimer(HighlightTimerInterval, DrawRectangles);
        }

        private void VideoPlayer_OnCompleted(object sender, VideoPlayerEventArgs e)
        {
            videoPlaying = false;
            highlightIndex = 0;
            VideoPlayer.Seek(-(int)Math.Ceiling(VideoPlayer.CurrentTime.TotalSeconds));
        }
    }
}
