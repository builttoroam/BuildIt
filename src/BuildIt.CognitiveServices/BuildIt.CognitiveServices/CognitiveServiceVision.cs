using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using PCLStorage;
using FileAccess = PCLStorage.FileAccess;
using System.Linq;
using BuildIt.CognitiveServices.Models;
using FaceRectangle = Microsoft.ProjectOxford.Face.Contract.FaceRectangle;

namespace BuildIt.CognitiveServices
{
    public class CognitiveServiceVision
    {

        /// <summary>
        /// Extract rich information from images to categorize and process visual data—and protect your users from unwanted content.
        /// </summary>
        /// <returns>
        /// Computer vision analysis result
        /// </returns>
        public async Task<ResultDto<AnalysisResult>> ComputerVisionApiRequestAsync(string subscriptionKey, string photoUri)
        {
            var resultDto = new ResultDto<AnalysisResult>();
            try
            {
                var photoStream = await PclStorageStreamAsync(photoUri);
                var analysisResult = await UploadAndAnalyzeImage(subscriptionKey, photoStream);

                resultDto.Result = analysisResult;
                resultDto.Success = resultDto.Result != null;
            }
            catch (Exception ex)
            {
                resultDto.ErrorMessage = ex.Message;
                resultDto.Exception = ex;
                Console.WriteLine($"Error: {ex}");
                
            }
            return resultDto;
        }

        private async Task<AnalysisResult> UploadAndAnalyzeImage(string subscriptionKey, Stream imageStream)
        {
            try
            {
                var visionServiceClient = new VisionServiceClient(subscriptionKey);
                using (var imageFileStream = imageStream)
                {
                    var visualFeatures = new VisualFeature[]
                    {
                        VisualFeature.Adult, VisualFeature.Categories, VisualFeature.Color, VisualFeature.Description,
                        VisualFeature.Faces, VisualFeature.ImageType, VisualFeature.Tags
                    };
                    var analysisResult = await visionServiceClient.AnalyzeImageAsync(imageFileStream, visualFeatures);
                    return analysisResult;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
                return null;
            }


        }

        private async Task<Stream> PclStorageStreamAsync(string uri)
        {
            try
            {
                var imageFile = await FileSystem.Current.GetFileFromPathAsync(uri);

                var photoStream = imageFile.OpenAsync(FileAccess.Read).Result;
                return photoStream;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong with image streaming. {ex}");
                return null;
            }
        }

        /// <summary>
        /// Analyze faces to detect a range of feelings and personalize your app's responses.
        /// </summary>
        /// <returns>
        /// Return emptionRects
        /// </returns>
        public async Task<ResultDto<Emotion[]>> VisionEmotionApiRequestAsync(string subscriptionKey, string photoUri)
        {
            var resultDto = new ResultDto<Emotion[]>();
            try
            {
                using (var photoStream = await PclStorageStreamAsync(photoUri))
                {
                    var emotionRects = await UploadAndDetectEmotion(subscriptionKey, photoStream);
                    resultDto.Result = emotionRects;
                    resultDto.Success = resultDto.Result != null;
                }
            }
            catch (Exception ex)
            {
                resultDto.ErrorMessage = ex.Message;
                resultDto.Exception = ex;
                Console.WriteLine($"{ex}");
                
            }
            return resultDto;
        }

        private async Task<Emotion[]> UploadAndDetectEmotion(string subscriptionKey, Stream imageStream)
        {
            try
            {
                var emotionServiceClient = new EmotionServiceClient(subscriptionKey);

                var emotionResult = await emotionServiceClient.RecognizeAsync(imageStream);
                return emotionResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error {ex}");
                return null;
            }
        }

        public async Task<ResultDto<FaceRectangle[]>> VisionFaceApiCheckAsync(string subscriptionKey, string photoUri)
        {
            var resultDto = new ResultDto<FaceRectangle[]>();
            try
            {
                using (var photoStream = await PclStorageStreamAsync(photoUri))
                {
                    var faceRects = await UploadAndDetectFaces(subscriptionKey, photoStream);
                    resultDto.Result = faceRects;
                    resultDto.Success = resultDto.Result != null;
                }
            }
            catch (Exception ex)
            {
                resultDto.ErrorMessage = ex.Message;
                Console.WriteLine($"Error: {ex}");
            }
            return resultDto;
        }

        private async Task<FaceRectangle[]> UploadAndDetectFaces(string subscriptionKey, Stream photoStream)
        {
            try
            {
                var faceServiceClient = new FaceServiceClient(subscriptionKey);
                var faces = await faceServiceClient.DetectAsync(photoStream);
                var faceRects = faces.Select(face => face.FaceRectangle);
                return faceRects.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error : {ex}");
                return new FaceRectangle[0];
            }
        }

    }
}
