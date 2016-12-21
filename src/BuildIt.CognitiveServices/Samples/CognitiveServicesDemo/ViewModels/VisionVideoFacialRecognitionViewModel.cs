﻿using CognitiveServicesDemo.Model;
using Microsoft.ProjectOxford.Video;
using Microsoft.ProjectOxford.Video.Contract;
using MvvmCross.Core.ViewModels;
using Newtonsoft.Json;
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
        private string videoPath;
        private string warningText;
        private string title;
        private List<FrameHighlight> frameHighlights = new List<FrameHighlight>();
        private double videoCurrentPosition;
        private string statusText;

        private static readonly TimeSpan QueryWaitTime = TimeSpan.FromSeconds(20);
        private VideoServiceClient VideoServiceClient { get; set; }

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

        public string WarningText
        {
            get { return warningText; }
            set
            {
                warningText = value;
                RaisePropertyChanged(() => WarningText);
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public List<FrameHighlight> FrameHighlights
        {
            get { return frameHighlights; }
            set
            {
                frameHighlights = value;
                RaisePropertyChanged(() => FrameHighlights);
            }
        }

        public double VideoCurrentPosition
        {
            get { return videoCurrentPosition; }
            set
            {
                videoCurrentPosition = value;
                RaisePropertyChanged(() => VideoCurrentPosition);
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


        public async Task UploadVideoAsync(MediaFile file)
        {
            Processing = true;
            VideoServiceClient = new VideoServiceClient("9739e652e7214256ac48cb85e641a96e")
            {
                Timeout = TimeSpan.FromMinutes(10)
            };

            //Operation videoOperation;
            try
            {
                //Stream testStream = file.GetStream();

                using (Stream videoStream = file.GetStream())
                {
                    StatusText = "Uploading Video";
                    var operation =
                        await VideoServiceClient.CreateOperationAsync(videoStream, new FaceDetectionOperationSettings());

                    OperationResult result = await VideoServiceClient.GetOperationResultAsync(operation);
                    while (result.Status != OperationStatus.Succeeded && result.Status != OperationStatus.Failed)
                    {
                        StatusText = "Waiting for Video to process";
                        Debug.WriteLine(
                            $"Server status: {result.Status}, wait {QueryWaitTime.TotalSeconds} seconds");
                        await Task.Delay(QueryWaitTime);
                        result = await VideoServiceClient.GetOperationResultAsync(operation);
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
                // ignored
            }

            Processing = false;
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
