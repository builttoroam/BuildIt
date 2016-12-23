using CognitiveServicesDemo.Model;
using Microsoft.ProjectOxford.Video;
using Microsoft.ProjectOxford.Video.Contract;
using MvvmCross.Core.ViewModels;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CognitiveServicesDemo.ViewModels
{
    public class VisionVideoFacialRecognitionViewModel : MvxViewModel
    {
        private static readonly TimeSpan QueryWaitTime = TimeSpan.FromSeconds(20);

        public IMvxCommand TakeVideoCommand { get; }

        private VideoServiceClient videoServiceClient;
        private string videoPath;
        private string statusText;

        public VisionVideoFacialRecognitionViewModel()
        {
            TakeVideoCommand = new MvxAsyncCommand(CaptureVideo);
        }

        public List<FrameHighlight> FrameHighlights { get; private set; } = new List<FrameHighlight>();
        public bool Processing { get; private set; }
        public double NaturalVideoWidth { get; private set; }
        public double NaturalVideoHeight { get; private set; }

        public string VideoPath
        {
            get { return videoPath; }
            set
            {
                videoPath = value;
                RaisePropertyChanged(() => VideoPath);
            }
        }

        public string StatusText
        {
            get { return statusText; }
            set
            {
                statusText = value;
                RaisePropertyChanged(() => StatusText);
            }
        }

        public async Task CaptureVideo()
        {
            try
            {
                await CrossMedia.Current.Initialize();
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported) return;

                var mediaOptions = new StoreVideoOptions
                {
                    Directory = "Media",
                    Name = $"{DateTime.Now:T}.mp4".Replace(":", "-")
                };

                var file = await CrossMedia.Current.TakeVideoAsync(mediaOptions);
                await UploadVideoAsync(file);
                VideoPath = file.Path;
            }
            catch (Exception ex)
            {
                StatusText = $"Exception of type: {ex.GetType().Name} ocurred with message: {ex.Message}";
            }
        }

        private async Task UploadVideoAsync(MediaFile file)
        {
            Processing = true;
            videoServiceClient = new VideoServiceClient("9739e652e7214256ac48cb85e641a96e")
            {
                Timeout = TimeSpan.FromMinutes(10)
            };

            try
            {
                using (Stream videoStream = file.GetStream())
                {
                    StatusText = "Uploading Video";
                    var operation = await videoServiceClient.CreateOperationAsync(videoStream, new FaceDetectionOperationSettings());

                    OperationResult result = await videoServiceClient.GetOperationResultAsync(operation);
                    while (result.Status != OperationStatus.Succeeded && result.Status != OperationStatus.Failed)
                    {
                        StatusText = $"Server status: {result.Status}, wait {QueryWaitTime.TotalSeconds} seconds";
                        Debug.WriteLine(StatusText);
                        await Task.Delay(QueryWaitTime);
                        result = await videoServiceClient.GetOperationResultAsync(operation);
                    }

                    StatusText = $"Finish processing with server status: {result.Status}";
                    Debug.WriteLine(StatusText);

                    // Processing finished, check result
                    if (result.Status == OperationStatus.Succeeded)
                    {
                        var faceTrackingResult = JsonConvert.DeserializeObject<FaceTracking>(result.ProcessingResult);
                        NaturalVideoHeight = faceTrackingResult.Height;
                        NaturalVideoWidth = faceTrackingResult.Width;
                        FrameHighlights = GetHighlights(result.ProcessingResult).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Exception of type: {ex.GetType().Name} ocurred with message: {ex.Message}";
            }
            finally
            {
                Processing = false;
            }
        }

        private static IEnumerable<FrameHighlight> GetHighlights(string json)
        {
            var faceTrackingResult = JsonConvert.DeserializeObject<FaceTracking>(json);

            if (faceTrackingResult.FacesDetected == null) yield break;

            var timescale = (float)faceTrackingResult.Timescale;
            var invisibleRect = new Rectangle(0, 0, 0, 0);

            foreach (var fragment in faceTrackingResult.Fragments)
            {
                var events = fragment.Events;
                if (events == null || events.Length == 0)
                {
                    var rects = new Rectangle[faceTrackingResult.FacesDetected.Length];
                    for (int i = 0; i < rects.Length; i++) rects[i] = invisibleRect;

                    yield return new FrameHighlight { Time = fragment.Start / timescale, HighlightRects = rects };
                }
                else
                {
                    var interval = fragment.Interval.GetValueOrDefault();
                    var start = fragment.Start;
                    var i = 0;
                    foreach (var evt in events)
                    {
                        var rects = faceTrackingResult.FacesDetected.Select(face =>
                        {
                            var faceRect = evt.FirstOrDefault(x => x.Id == face.FaceId);
                            if (faceRect == null) return invisibleRect;

                            return new Rectangle(faceRect.X, faceRect.Y, faceRect.Width, faceRect.Height);
                        }).ToArray();

                        yield return new FrameHighlight { Time = (start + interval * i) / timescale, HighlightRects = rects };

                        i++;
                    }
                }
            }
        }
    }
}
