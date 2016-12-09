using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using PCLStorage;
using Xamarin.Forms;
using Microsoft.ProjectOxford.Face;
using CognitiveServicesDemo.Common;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using FaceRectangle = Microsoft.ProjectOxford.Face.Contract.FaceRectangle;
using Image = Xamarin.Forms.Image;
using Plugin.Media.Abstractions;
using BuildIt.CognitiveServices;

namespace CognitiveServicesDemo.ViewModels
{
    public class VisionFaceViewModel : MvxViewModel
    {
        private FaceServiceClient FaceServiceClient { get; set; }
        private EmotionServiceClient EmotionServiceClient { get; set; }
        private string title;
        private string analysisCategories;
        private string descriptionCaptions;
        private string analysisFaces;
        private string analysisTag;
        private string imageUrl;
        private string warningText;
        //private readonly IPhotoPropertiesService photoPropertiesService;
        private string imageMetadata;
        private Microsoft.ProjectOxford.Vision.Contract.Face[] faceMetadata;
        private string[] xywh;

        //readonly CognitiveServiceVision cognitiveServiceVision = new CognitiveServiceVision();
        private double x = 0.25;
        private double y = 0.28;
        private double width = 0.4;
        private double height = 0.4;

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public string ImageUrl
        {
            get { return imageUrl; }
            set
            {
                imageUrl = value;
                RaisePropertyChanged(() => ImageUrl);
            }
        }

        public Microsoft.ProjectOxford.Vision.Contract.Face[] FaceMetadata
        {
            get { return faceMetadata; }
            set
            {
                faceMetadata = value;
                RaisePropertyChanged(() => FaceMetadata);
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

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        public double Width
        {
            get { return width; }
            set { width = value; }
        }

        public double Height
        {
            get { return height; }
            set { height = value; }
        }


        public VisionFaceViewModel()
        {
           
        }


        public async Task VisionFaceCheckAsync(MediaFile file)
        {
            //var filePath = "Assets/carre_homme.jpg";
            //Image image = new Image();
            //image.Source = filePath;

            //ImageUrl = filePath;
            //var photoStream = await PclStorageStreamAsync(filePath);
            if (string.IsNullOrEmpty(ImageUrl))
            {
                WarningText = "Please take photo first";
            }
            else
            {
                var computerVision = new FaceAPIV10();
                var result = await computerVision.FaceDetectWithHttpMessagesAsync(file.GetStream(), null, null, null, null, Constants.FaceApiKey);
            }
        }

        public async Task VisionEmotionAsync(MediaFile file)
        {
            if (string.IsNullOrEmpty(ImageUrl))
            {
                WarningText = "Please take photo first";
            }
            else
            {
                Title = "Checking image";
                var emotion = new EmotionAPI();
                var result = await emotion.EmotionRecognitionWithHttpMessagesAsync(file.GetStream(), null,Constants.EmotionApiKey);

                var faceNo = 1;
                
                var value = "";
                //var test = await cognitiveServiceVision.VisionEmotionApiRequestAsync(Constants.EmotionApiKey, ImageUrl);
                //Emotion[] emotionRects = await cognitiveServiceVision.VisionEmotionApiRequestAsync(Constants.FaceApiKey,ImageUrl);
                //var photoStream = await PclStorageStreamAsync(ImageUrl);
                //Emotion[] emotionRects = await UploadAndDetectEmotion(await PclStorageStreamAsync(ImageUrl));

                if (result != null && result.Length > 0)
                {
                    Title = "Here is the result";
                    foreach (var emotionRect in result)
                    {
                        value +=
                            $"Face{faceNo} Anger: {emotionRect.Scores.Anger} Contempt: {emotionRect.Scores.Contempt} Disgust: {emotionRect.Scores.Disgust} Fear: {emotionRect.Scores.Fear} Happiness: {emotionRect.Scores.Happiness} Neutral: {emotionRect.Scores.Neutral} Sadness: {emotionRect.Scores.Sadness} Surprise: {emotionRect.Scores.Surprise}";
                        faceNo++;
                    }
                }
                else
                {
                    value = "No results";
                }
                CleanResult();
                Title = value;
            }
        }

        public async Task VisionComputerVisionAsync(MediaFile file)
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
                
                //var analysisCategories = string.Empty;
                //var descriptionCaptions = string.Empty;
                //var analysisFaces = string.Empty;
                //var analysisTag = string.Empty;

                ////call from class library
                //var co = new CognitiveServiceClient();
                //var result = await co.ComputerVisionApiRequestAsync(Constants.CuomputerVisionApiKey, file.GetStream());
                ////var photoStream = await PclStorageStreamAsync(ImageUrl);
                //AnalysisResult analysisRects = await UploadAndAnalyzeImage(file.GetStream());
                var faceNo = 1;
                if (analysisRects != null)
                {
                    Title = "Here is the result";
                    foreach (var analysisRect in analysisRects.Categories)
                    {
                        analysisCategories += $" {analysisRect.Name} + Score: {analysisRect.Score}";
                    }

                    foreach (var descriptionCaption in analysisRects.Description.Captions)
                    {
                        descriptionCaptions += $"{descriptionCaption.Text} + Confidence :{descriptionCaption.Confidence}";
                    }
                    if (analysisRects.Faces.Length >= 1)
                    {
                        Xywh = new string[analysisRects.Faces.Length];
                        for (int i = 0; i < analysisRects.Faces.Length; i++)
                        {
                            
                            Xywh[i] =
                                $"{analysisRects.Faces[i].FaceRectangle.Left},{analysisRects.Faces[i].FaceRectangle.Top},{analysisRects.Faces[i].FaceRectangle.Width},{analysisRects.Faces[i].FaceRectangle.Height}";
                        }

                        //Xywh = new string[analysisRects.Faces.Length];
                        //foreach (var face in analysisRects.Faces)
                        //{
                        //    Xywh.Add($"{face.FaceRectangle.Left},{face.FaceRectangle.Top},{face.FaceRectangle.Width},{face.FaceRectangle.Height}");
                        //    //Xywh = $"{analysisRects.Faces[0].FaceRectangle.Left},{analysisRects.Faces[0].FaceRectangle.Top},{analysisRects.Faces[0].FaceRectangle.Width},{face.FaceRectangle.Height}";
                        //}
                        //Xywh = $"{analysisRects.Faces[0].FaceRectangle.Left},{analysisRects.Faces[0].FaceRectangle.Top},{analysisRects.Faces[0].FaceRectangle.Width},{analysisRects.Faces[0].FaceRectangle.Height}";
                    }
                    else
                    {
                        WarningText = "Can't detect face, please take another photo";
                        Xywh = new string[1];
                        Xywh[0] = ("0,0,0,0");
                        
                    }

                    FaceMetadata = analysisRects.Faces;
                    foreach (var face in analysisRects.Faces)
                    {
                        analysisFaces += $"FaceNo: {faceNo} Age: {face.Age} + Gender: {face.Gender}";

                        //FaceMetadata = new List<string>
                        //{
                        //    $"{face.FaceRectangle.Width},{face.FaceRectangle.Height},{face.FaceRectangle.Left},{face.FaceRectangle.Top}"
                        //};
                        //Xywh = $"{face.FaceRectangle.Left},{face.FaceRectangle.Top},{face.FaceRectangle.Width},{face.FaceRectangle.Height}";

                        faceNo++;
                    }
                    foreach (var analysisRectsTag in analysisRects.Tags)
                    {
                        analysisTag += $"Confident: {analysisRectsTag.Confidence} + Name: {analysisRectsTag.Name}";
                    }

                    ImageMetadata = $"{analysisRects.Metadata.Width},{analysisRects.Metadata.Height}";

                    AnalysisCategories = analysisCategories;
                    DescriptionCaptions = descriptionCaptions;
                    AnalysisFaces = analysisFaces;
                    AnalysisTag = analysisTag;

                    var value = $"Adult content: {analysisRects.Adult.IsAdultContent}, Image infor: {analysisRects.Metadata.Format} + Height: {analysisRects.Metadata.Height} + Width: {analysisRects.Metadata.Width}";
                    Title = value;
                }
            }

        }

        private async Task CreateRectangle()
        {
            BoxView boxView = new BoxView
            {

            };

            AbsoluteLayout absoluteLayout = new AbsoluteLayout
            {


                Children =
                {

                }
            };

            //AbsoluteLayout.SetLayoutBounds(boxView, new Rectangle(100f, 200f, 200f, 50f));

        }

        private async Task<AnalysisResult> UploadAndAnalyzeImage(Stream imageStream)
        {
            var visionServiceClient = new VisionServiceClient(Constants.CuomputerVisionApiKey);
            var assembley = this.GetType().GetTypeInfo().Assembly;
            using (Stream imageFileStream = imageStream)
            {
                VisualFeature[] visualFeatures = new VisualFeature[]
                {
                    VisualFeature.Adult, VisualFeature.Categories, VisualFeature.Color, VisualFeature.Description,
                    VisualFeature.Faces, VisualFeature.ImageType, VisualFeature.Tags
                };
                AnalysisResult analysisResult =
                    await visionServiceClient.AnalyzeImageAsync(imageFileStream, visualFeatures);
                return analysisResult;
            }

        }


        public async Task<Stream> PclStorageStreamAsync(string url)
        {
            try
            {
                IFile imageFile = await FileSystem.Current.GetFileFromPathAsync(url);

                var photoStream = imageFile.OpenAsync(FileAccess.Read).Result;
                var webImage = new Image { Aspect = Aspect.AspectFit };
                webImage.Source = ImageSource.FromUri(new Uri(url));
                return photoStream;

            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<FaceRectangle[]> UploadAndDetectFaces(Stream photoStream)
        {
            FaceServiceClient = new FaceServiceClient(Constants.FaceApiKey);
            try
            {
                using (Stream imageFileStream = photoStream)
                {
                    var faces = await FaceServiceClient.DetectAsync(imageFileStream);
                    var faceRects = faces.Select(face => face.FaceRectangle);
                    return faceRects.ToArray();
                }
            }
            catch (Exception ex)
            {
                return new FaceRectangle[0];
            }
        }

        private async Task<Emotion[]> UploadAndDetectEmotion(Stream imageStream)
        {
            EmotionServiceClient = new EmotionServiceClient(Constants.EmotionApiKey);
            try
            {
                Emotion[] emotionResult;
                using (Stream imageFileStream = imageStream)
                {
                    emotionResult = await EmotionServiceClient.RecognizeAsync(imageFileStream);
                    return emotionResult;
                }
            }
            catch (Exception ex)
            {

                return new Emotion[0];
            }
        }

        private void CleanResult()
        {
            AnalysisCategories = string.Empty;
            DescriptionCaptions = string.Empty;
            AnalysisFaces = string.Empty;
            AnalysisTag = string.Empty;
        }
    }
}
