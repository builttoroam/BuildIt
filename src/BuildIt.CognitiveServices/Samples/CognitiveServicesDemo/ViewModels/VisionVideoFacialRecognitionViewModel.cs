using Microsoft.ProjectOxford.Video;
using Microsoft.ProjectOxford.Video.Contract;
using MvvmCross.Core.ViewModels;
using Newtonsoft.Json;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CognitiveServicesDemo.ViewModels
{
    public class VisionVideoFacialRecognitionViewModel : MvxViewModel
    {
        private string videoPath;
        private string warningText;
        private string title;
        private List<Rectangle> rectangles = new List<Rectangle>();
        private double videoCurrentPosition;

        private static readonly TimeSpan QueryWaitTime = TimeSpan.FromSeconds(20);
        private VideoServiceClient VideoServiceClient { get; set; }

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

        public List<Rectangle> Rectangles
        {
            get { return rectangles; }
            set
            {
                rectangles = value;
                RaisePropertyChanged(() => Rectangles);
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


        public async void UploadVideoAsync(MediaFile file)
        {

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
                    var operation = await VideoServiceClient.CreateOperationAsync(videoStream, new FaceDetectionOperationSettings());

                    OperationResult result = await VideoServiceClient.GetOperationResultAsync(operation);
                    while (result.Status != OperationStatus.Succeeded && result.Status != OperationStatus.Failed)
                    {
                        Debug.WriteLine(
                            $"Server status: {result.Status}, wait {QueryWaitTime.TotalSeconds} seconds");
                        await Task.Delay(QueryWaitTime);
                        result = await VideoServiceClient.GetOperationResultAsync(operation);
                    }
                    Debug.WriteLine($"Finish processing with server status: {result.Status}");

                    // Processing finished, check result
                    if (result.Status == OperationStatus.Succeeded)
                    {
                        var rec = new Rectangle();

                        var videoAnalysisResult =
                            JsonConvert.DeserializeObject<FaceTracking>(result.ProcessingResult);
                        foreach (var fragment in videoAnalysisResult.Fragments)
                        {
                            foreach (var fragmentEvent in fragment.Events)
                            {
                                foreach (var faceEvent in fragmentEvent)
                                {
                                    rec.X = faceEvent.X;
                                    rec.Y = faceEvent.Y;
                                    rec.Width = faceEvent.Width;
                                    rec.Height = faceEvent.Height;
                                    Rectangles.Add(rec);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // ignored
            }
        }
    }
}
