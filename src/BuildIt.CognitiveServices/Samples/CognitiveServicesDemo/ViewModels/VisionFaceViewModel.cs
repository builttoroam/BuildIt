using BuildIt.CognitiveServices;
using BuildIt.CognitiveServices.Interfaces;
using BuildIt.CognitiveServices.Models;
using CognitiveServicesDemo.Common;
using ExifLib;
using MvvmCross.Core.ViewModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CognitiveServicesDemo.ViewModels
{
    public class VisionFaceViewModel : MvxViewModel
    {
        private ICognitiveServiceClient CognitiveServiceClient { get; } = new CognitiveServiceClient();

        private string title;
        private string analysisCategories;
        private string descriptionCaptions;
        private string analysisFaces;
        private string analysisTag;
        private string warningText;
        private string imageMetadata;
        private string[] xywh;
        private ImageSource imageSource;
        private double naturalImageWidth;
        private double naturalImageHeight;
        private List<FaceRectangle> faceRectangles;

        public VisionFaceViewModel()
        {
            TakePhotoCommand = new MvxAsyncCommand(TakePhoto);
        }

        public IMvxCommand TakePhotoCommand { get; }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public string AnalysisCategories
        {
            get { return analysisCategories; }
            set
            {
                analysisCategories = value;
                RaisePropertyChanged(() => AnalysisCategories);
            }
        }

        public string DescriptionCaptions
        {
            get { return descriptionCaptions; }
            set
            {
                descriptionCaptions = value;
                RaisePropertyChanged(() => DescriptionCaptions);
            }
        }

        public string AnalysisFaces
        {
            get { return analysisFaces; }
            set
            {
                analysisFaces = value;
                RaisePropertyChanged(() => AnalysisFaces);
            }
        }

        public string AnalysisTag
        {
            get { return analysisTag; }
            set
            {
                analysisTag = value;
                RaisePropertyChanged(() => AnalysisTag);
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

        public string ImageMetadata
        {
            get { return imageMetadata; }
            set
            {
                imageMetadata = value;
                RaisePropertyChanged(() => ImageMetadata);
            }
        }

        public string[] Xywh
        {
            get { return xywh; }
            set
            {
                xywh = value;
                RaisePropertyChanged(() => Xywh);
            }
        }

        public ImageSource ImageSource
        {
            get { return imageSource; }
            set
            {
                imageSource = value;
                RaisePropertyChanged(() => ImageSource);
            }
        }

        public double NaturalImageWidth
        {
            get { return naturalImageWidth; }
            set
            {
                naturalImageWidth = value;
                RaisePropertyChanged(() => NaturalImageWidth);
            }
        }

        public double NaturalImageHeight
        {
            get { return naturalImageHeight; }
            set
            {
                naturalImageHeight = value;
                RaisePropertyChanged(() => NaturalImageHeight);
            }
        }

        public List<FaceRectangle> FaceRectangles
        {
            get { return faceRectangles; }
            set
            {
                faceRectangles = value;
                RaisePropertyChanged(() => FaceRectangles);
            }
        }

        private async Task TakePhoto()
        {
            try
            {
                await CrossMedia.Current.Initialize();
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported) return;

                // Supply media options for saving our photo after it's taken.
                var mediaOptions = new StoreCameraMediaOptions
                {
                    Directory = "Media",
                    Name = $"{DateTime.Now:T}.jpg".Replace(":", "-"),
                    DefaultCamera = CameraDevice.Front,
                    SaveToAlbum = false
                };

                // Take a photo of the business receipt.
                var file = await CrossMedia.Current.TakePhotoAsync(mediaOptions);

                if (file == null)
                {
                    WarningText = "Please take photo first";
                    return;
                }

                Title = "Uploading and processing image";
                var analysisResult = await CognitiveServiceClient.ComputerVisionApiRequestAsync(Constants.CuomputerVisionApiKey, file.GetStream());

                var list = new List<FaceRectangle>();
                var imageWidth = 0d;
                var imageHeight = 0d;

                if (analysisResult.Success)
                {
                    AnalysisCategories = string.Empty;
                    DescriptionCaptions = string.Empty;
                    AnalysisFaces = string.Empty;
                    AnalysisTag = string.Empty;
                    var analysis = analysisResult.Result;

                    foreach (var analysisCategory in analysis.Categories)
                    {
                        AnalysisCategories += $" {analysisCategory.Name} + Score: {analysisCategory.Score}";
                    }

                    foreach (var descriptionCaption in analysis.Description.Captions)
                    {
                        DescriptionCaptions += $"{descriptionCaption.Text} + Confidence :{descriptionCaption.Confidence}";
                    }

                    var faceNo = 1;
                    var rectangle = new FaceRectangle();
                    foreach (var analysisFace in analysis.Faces)
                    {
                        rectangle.Y = analysisFace.FaceRectangle.Top;
                        rectangle.X = analysisFace.FaceRectangle.Left;
                        rectangle.Width = analysisFace.FaceRectangle.Width;
                        rectangle.Height = analysisFace.FaceRectangle.Height;

                        list.Add(rectangle);

                        AnalysisFaces += $"FaceNo: {faceNo} Age: {analysisFace.Age} + Gender: {analysisFace.Gender}";
                        faceNo++;
                    }

                    foreach (var tag in analysis.Tags)
                    {
                        AnalysisTag += $"Confident: {tag.Confidence} + Name: {tag.Name}";
                    }

                    ImageMetadata = $"{analysis.Metadata.Width},{analysis.Metadata.Height}";
                    imageHeight = analysis.Metadata.Height;
                    imageWidth = analysis.Metadata.Width;

                    Title = $"Adult content: {analysis.Adult.IsAdultContent}, Image infor: {analysis.Metadata.Format} + Height: {analysis.Metadata.Height} + Width: {analysis.Metadata.Width}";
                }

                using (var streamPic = file.GetStream())
                {
                    var picInfo = ExifReader.ReadJpeg(streamPic);
                    var orientation = picInfo.Orientation;
                    if ((orientation == ExifOrientation.BottomLeft || orientation == ExifOrientation.TopRight)
                        && imageHeight < imageWidth) // Orientation is wrong?
                    {
                        var temp = imageHeight;
                        imageHeight = imageWidth;
                        imageWidth = temp;
                    }
                }

                ImageSource = file.Path;
                NaturalImageWidth = imageWidth;
                NaturalImageHeight = imageHeight;
                FaceRectangles = list;

                WarningText = "Here is the computer vision results for you";
            }
            catch (Exception ex)
            {
                WarningText = $"Exception of type: {ex.GetType().Name} ocurred with message: {ex.Message}";
            }
        }
    }
}
