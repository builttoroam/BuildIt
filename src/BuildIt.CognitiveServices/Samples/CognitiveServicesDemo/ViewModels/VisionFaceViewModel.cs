using BuildIt.CognitiveServices;
using CognitiveServicesDemo.Common;
using ExifLib;
using MvvmCross.Core.ViewModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CognitiveServicesDemo.ViewModels
{
    public class VisionFaceViewModel : MvxViewModel
    {
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
        private List<Rectangle> faceRectangles;

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

        public List<Rectangle> FaceRectangles
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

                await VisionComputerVisionAsync(file);
                var faceMetaData = Xywh;

                var list = new List<Rectangle>();
                var rectangle = new Rectangle();
                foreach (var s in faceMetaData)
                {
                    if (s != null)
                    {
                        var currentXywh = s.Split(',');

                        rectangle.X = double.Parse(currentXywh[0]);
                        rectangle.Y = double.Parse(currentXywh[1]);
                        rectangle.Width = double.Parse(currentXywh[2]);
                        rectangle.Height = double.Parse(currentXywh[3]);

                        list.Add(rectangle);
                    }
                }

                var metadata = ImageMetadata.Split(',');
                var imageWidth = int.Parse(metadata[0]);
                var imageHeight = int.Parse(metadata[1]);
                Debug.WriteLine($"current metadata\n{ImageMetadata}");

                // Do some stuff to make sure the boxes render correctly
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

        private async Task VisionComputerVisionAsync(MediaFile file)
        {
            if (file == null)
            {
                WarningText = "Please take photo first";
            }
            else
            {
                Title = "Checking image";
                var computerVision = new ComputerVisionAPIV10();
                var analysisRects = await computerVision.AnalyzeImageWithHttpMessagesAsync(file.GetStream(), "Categories", null, "en", null,
                    Constants.CuomputerVisionApiKey);

                AnalysisCategories = string.Empty;
                DescriptionCaptions = string.Empty;
                AnalysisFaces = string.Empty;
                AnalysisTag = string.Empty;

                var faceNo = 1;
                if (analysisRects != null)
                {
                    Title = "Here is the result";
                    foreach (var analysisRect in analysisRects.Categories)
                    {
                        AnalysisCategories += $" {analysisRect.Name} + Score: {analysisRect.Score}";
                    }

                    foreach (var descriptionCaption in analysisRects.Description.Captions)
                    {
                        DescriptionCaptions += $"{descriptionCaption.Text} + Confidence :{descriptionCaption.Confidence}";
                    }
                    if (analysisRects.Faces.Length >= 1)
                    {
                        Xywh = new string[analysisRects.Faces.Length];
                        for (int i = 0; i < analysisRects.Faces.Length; i++)
                        {

                            Xywh[i] =
                                $"{analysisRects.Faces[i].FaceRectangle.Left},{analysisRects.Faces[i].FaceRectangle.Top},{analysisRects.Faces[i].FaceRectangle.Width},{analysisRects.Faces[i].FaceRectangle.Height}";
                        }
                    }
                    else
                    {
                        WarningText = "Can't detect face, please take another photo";
                        Xywh = new string[1];
                        Xywh[0] = "0,0,0,0";

                    }

                    foreach (var face in analysisRects.Faces)
                    {
                        AnalysisFaces += $"FaceNo: {faceNo} Age: {face.Age} + Gender: {face.Gender}";
                        faceNo++;
                    }
                    foreach (var analysisRectsTag in analysisRects.Tags)
                    {
                        AnalysisTag += $"Confident: {analysisRectsTag.Confidence} + Name: {analysisRectsTag.Name}";
                    }

                    ImageMetadata = $"{analysisRects.Metadata.Width},{analysisRects.Metadata.Height}";

                    var value = $"Adult content: {analysisRects.Adult.IsAdultContent}, Image infor: {analysisRects.Metadata.Format} + Height: {analysisRects.Metadata.Height} + Width: {analysisRects.Metadata.Width}";
                    Title = value;
                }
            }

        }
    }
}
