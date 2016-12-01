using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BuildIt.CognitiveServices.Common;
using BuildIt.CognitiveServices.Interfaces;
using BuildIt.CognitiveServices.Models;
using BuildIt.CognitiveServices.Models.Feeds.Academic;
using BuildIt.CognitiveServices.Models.Feeds.InputParameters;
using BuildIt.CognitiveServices.Models.Feeds.Language;
using BuildIt.CognitiveServices.Models.Feeds.Search;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Newtonsoft.Json;
using FaceRectangle = Microsoft.ProjectOxford.Face.Contract.FaceRectangle;

namespace BuildIt.CognitiveServices
{
    public class CognitiveServiceClient : ICognitiveServiceClient
    {
        public async Task<ResultDto<InterpretApiFeeds>> AcademicInterpretApiRequestAsync(AcademicParameters academicParameters)
        {
            var resultDto = new ResultDto<InterpretApiFeeds>();
            try
            {
                var client = new HttpClient();

                //request header
                client.DefaultRequestHeaders.Add(Constants.SubscriptionTitle, academicParameters.SubscriptionKey);
                var queryString = $"query={academicParameters.Query}&offset={academicParameters.Offset}&count={academicParameters.Count}&model={academicParameters.Model}&complete={academicParameters.Complete}&timeout={academicParameters.Timeout}";
                var url = Constants.AcademicInterpretApi + queryString;

                var jsonResult = await client.GetStringAsync(url);
                var feed = JsonConvert.DeserializeObject<InterpretApiFeeds>(jsonResult);

                resultDto.Result = feed;
                resultDto.ErrorMessage = feed.error?.message;
                resultDto.StatusCode = feed.error?.code;
                resultDto.Success = feed.error == null;
            }
            catch (Exception ex)
            {
                resultDto.ErrorMessage = ex.Message;
                resultDto.Exception = ex;
                Debug.WriteLine($"{ex}");
            }
            return resultDto;
        }

        public async Task<ResultDto<SpellCheckApiFeeds>> SpellCheckApiRequestAsync(SpellCheckParameters spellCheckParameters)
        {
            var resultDto = new ResultDto<SpellCheckApiFeeds>();
            try
            {
                var client = new HttpClient();
                // Request headers
                client.DefaultRequestHeaders.Add(Constants.SubscriptionTitle, spellCheckParameters.subscriptionKey);

                // Request parameters
                var queryString = $"text={spellCheckParameters.content}&mode={spellCheckParameters.mode}&mkt={spellCheckParameters.mkt}";
                var url = Constants.SpellCheckApi + queryString;

                var jsonResult = await client.GetStringAsync(url);
                var feed = JsonConvert.DeserializeObject<SpellCheckApiFeeds>(jsonResult);
                resultDto.Result = feed;
                resultDto.ErrorMessage = feed.message;
                resultDto.StatusCode = feed.statusCode.ToString();
                resultDto.Success = string.IsNullOrEmpty(feed.message);
            }
            catch (Exception ex)
            {
                resultDto.ErrorMessage = ex.Message;
                resultDto.Exception = ex;
                Debug.WriteLine($"Error: {ex}");
            }
            return resultDto;
        }

        public async Task<ResultDto<DetectLanguageApiFeeds>> DetectLanguageApiRequestAsync(string subscriptionKey, string jsonString, string contentType = Constants.DefaultContentType, int numberofLanguagesToDetect = 1)
        {
            var resultDto = new ResultDto<DetectLanguageApiFeeds>();
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add(Constants.SubscriptionTitle, subscriptionKey);
                //request parameters
                var parameter = $"numberOfLanguagesToDetect={numberofLanguagesToDetect}";
                var url = Constants.DetectLanguageApi + parameter;
                HttpResponseMessage response;

                // Request body
                var byteData = Encoding.UTF8.GetBytes(jsonString);

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    response = await client.PostAsync(url, content);
                }
                if (!response.IsSuccessStatusCode || (response.Content == null)) return null;
                var jsonResult = await response.Content.ReadAsStringAsync();
                var feed = JsonConvert.DeserializeObject<DetectLanguageApiFeeds>(jsonResult);

                resultDto.Result = feed;
                resultDto.ErrorMessage = feed.message;
                resultDto.StatusCode = feed.statusCode.ToString();
                foreach (var error in feed.errors)
                {
                    resultDto.ErrorMessage += $"InnerError :{error.message}";
                    resultDto.StatusCode += $"InnerErrorCode :{error.id}";
                }

                resultDto.Success = string.IsNullOrEmpty(feed.message);
            }
            catch (Exception ex)
            {
                resultDto.ErrorMessage = ex.Message;
                resultDto.Exception = ex;
                Debug.WriteLine($"Error: {ex}");
            }
            return resultDto;
        }

        public async Task<ResultDto<KeyPhrasesApiFeeds>> KeyPhrasesApiRequestAsync(string subscriptionKey, string jsonString, string contentType = Constants.DefaultContentType)
        {
            var resultDto = new ResultDto<KeyPhrasesApiFeeds>();
            try
            {
                var client = new HttpClient();
                HttpResponseMessage response;

                // Request body
                var byteData = Encoding.UTF8.GetBytes(jsonString);

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    response = await client.PostAsync(Constants.KeyPhrasesApi, content);
                }
                if (!response.IsSuccessStatusCode || (response.Content == null)) return null;
                var jsonResult2 = await response.Content.ReadAsStringAsync();
                var feed = JsonConvert.DeserializeObject<KeyPhrasesApiFeeds>(jsonResult2);

                resultDto.Result = feed;
                resultDto.ErrorMessage = feed.message;
                resultDto.StatusCode = feed.statusCode.ToString();
                foreach (var error in feed.errors)
                {
                    resultDto.ErrorMessage += $"InnerError :{error.message}";
                    resultDto.StatusCode += $"InnerErrorCode :{error.id}";
                }
                resultDto.Success = string.IsNullOrEmpty(feed.message);
            }
            catch (Exception ex)
            {
                resultDto.ErrorMessage = ex.Message;
                resultDto.Exception = ex;
                Debug.WriteLine($"Error: {ex}");
            }
            return resultDto;
        }

        public async Task<ResultDto<SentimentApiFeeds>> SentimentApiRequestAsync(string subscriptionKey, string jsonString, string contentType = Constants.DefaultContentType)
        {
            var resultDto = new ResultDto<SentimentApiFeeds>();
            try
            {
                var client = new HttpClient();
                HttpResponseMessage response;

                // Request body
                var byteData = Encoding.UTF8.GetBytes(jsonString);

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    response = await client.PostAsync(Constants.SentimentApi, content);
                }
                var jsonResult = await response.Content.ReadAsStringAsync();
                var feed = JsonConvert.DeserializeObject<SentimentApiFeeds>(jsonResult);

                resultDto.Result = feed;
                resultDto.ErrorMessage = feed.message;
                resultDto.StatusCode = feed.statusCode.ToString();
                foreach (var error in feed.errors)
                {
                    resultDto.ErrorMessage += $"InnerError :{error.message}";
                    resultDto.StatusCode += $"InnerErrorCode :{error.id}";
                }
                resultDto.Success = string.IsNullOrEmpty(feed.message);
            }
            catch (Exception ex)
            {
                resultDto.ErrorMessage = ex.Message;
                resultDto.Exception = ex;
                Debug.WriteLine($"Error: {ex}");
            }
            return resultDto;
        }

        public async Task<ResultDto<BreakIntoWordsApiFeeds>> BreakIntoWordsApiRequestAsync(BreakIntoWordsParameters breakIntoWordsParameters)
        {
            var resultDto = new ResultDto<BreakIntoWordsApiFeeds>();
            try
            {
                var client = new HttpClient();

                //request header
                client.DefaultRequestHeaders.Add(Constants.SubscriptionTitle, breakIntoWordsParameters.subscriptionKey);
                var queryString = $"model={breakIntoWordsParameters.model}&text={breakIntoWordsParameters.text}&order={breakIntoWordsParameters.order}&maxNumOfCandidatesReturned={breakIntoWordsParameters.maxNumOfCandidatesReturned}";
                var body = string.Empty;
                var uri = Constants.BreakIntoWordsApi + queryString;

                HttpResponseMessage response;
                //request body
                var byteData = Encoding.UTF8.GetBytes(body);

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue(breakIntoWordsParameters.content);
                    response = await client.PostAsync(uri, content);
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                var feed = JsonConvert.DeserializeObject<BreakIntoWordsApiFeeds>(jsonResult);

                resultDto.Result = feed;
                resultDto.ErrorMessage = feed.error.message;
                resultDto.StatusCode = feed.error.code;
                resultDto.Success = feed.error == null;
            }
            catch (Exception ex)
            {
                resultDto.ErrorMessage = ex.Message;
                resultDto.Exception = ex;
                Debug.WriteLine($"{ex}");
            }
            return resultDto;
        }

        public async Task<ResultDto<BingAutosuggestApiFeeds>> BingAutosuggestApiRequestAsync(string subscriptionKey, string context)
        {
            var resultDto = new ResultDto<BingAutosuggestApiFeeds>();
            try
            {
                var client = new HttpClient();

                //request header
                client.DefaultRequestHeaders.Add(Constants.SubscriptionTitle, subscriptionKey);
                var queryString = $"q={context}";
                var url = Constants.BingAutosuggestAPi + queryString;

                var jsonResult = await client.GetStringAsync(url);
                var feed = JsonConvert.DeserializeObject<BingAutosuggestApiFeeds>(jsonResult);

                resultDto.Result = feed;
                resultDto.ErrorMessage = feed.message;
                resultDto.StatusCode = feed.statusCode.ToString();
                resultDto.Success = string.IsNullOrEmpty(feed.message);
            }
            catch (Exception ex)
            {
                resultDto.ErrorMessage = ex.Message;
                resultDto.Exception = ex;
                Debug.WriteLine($"{ex}");
            }
            return resultDto;
        }

        public async Task<ResultDto<BingSearchApiFeeds>> BingSearchApiRequestAsync(BingSearchParameters bingSearchParameters)
        {
            var resultDto = new ResultDto<BingSearchApiFeeds>();
            try
            {
                var client = new HttpClient();

                //request header
                client.DefaultRequestHeaders.Add(Constants.SubscriptionTitle, bingSearchParameters.subscriptionKey);
                var queryString = $"q={bingSearchParameters.content}&count={bingSearchParameters.count}&offset={bingSearchParameters.offset}&mkt={bingSearchParameters.mkt}&safesearch={bingSearchParameters.safesearch}";
                var url = Constants.BingSearchApi + queryString;

                var jsonResult = await client.GetStringAsync(url);
                var feed = JsonConvert.DeserializeObject<BingSearchApiFeeds>(jsonResult);

                resultDto.Result = feed;
                resultDto.ErrorMessage = feed.message;
                resultDto.StatusCode = feed.statusCode.ToString();
                resultDto.Success = string.IsNullOrEmpty(feed.message);
            }
            catch (Exception ex)
            {
                resultDto.Exception = ex;
                Debug.WriteLine($"{ex}");
            }
            return resultDto;
        }

        /// <summary>
        ///     Extract rich information from images to categorize and process visual data—and protect your users from unwanted
        ///     Query.
        /// </summary>
        /// <returns>
        ///     Computer vision analysis result
        /// </returns>
        public async Task<ResultDto<AnalysisResult>> ComputerVisionApiRequestAsync(string subscriptionKey, Stream photoStream)
        {
            var resultDto = new ResultDto<AnalysisResult>();
            try
            {
                var analysisResult = await UploadAndAnalyzeImage(subscriptionKey, photoStream);

                resultDto.Result = analysisResult;
                resultDto.Success = resultDto.Result != null;
            }
            catch (Exception ex)
            {
                resultDto.ErrorMessage = ex.Message;
                resultDto.Exception = ex;
                Debug.WriteLine($"Error: {ex}");
            }
            return resultDto;
        }

        /// <summary>
        ///     Analyze faces to detect a range of feelings and personalize your app's responses.
        /// </summary>
        /// <returns>
        ///     Return emptionRects
        /// </returns>
        public async Task<ResultDto<Emotion[]>> VisionEmotionApiRequestAsync(string subscriptionKey, Stream photoStream)
        {
            var resultDto = new ResultDto<Emotion[]>();
            try
            {
                using (photoStream)
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
                Debug.WriteLine($"{ex}");
            }
            return resultDto;
        }

        public async Task<ResultDto<FaceRectangle[]>> VisionFaceApiCheckAsync(string subscriptionKey, Stream photoStream)
        {
            var resultDto = new ResultDto<FaceRectangle[]>();
            try
            {
                using (photoStream)
                {
                    var faceRects = await UploadAndDetectFaces(subscriptionKey, photoStream);
                    resultDto.Result = faceRects;
                    resultDto.Success = resultDto.Result != null;
                }
            }
            catch (Exception ex)
            {
                resultDto.ErrorMessage = ex.Message;
                Debug.WriteLine($"Error: {ex}");
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
                    var visualFeatures = new[]
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
                Debug.WriteLine($"{ex}");
                return null;
            }
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
                Debug.WriteLine($"Error {ex}");
                return null;
            }
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
                Debug.WriteLine($"Error : {ex}");
                return new FaceRectangle[0];
            }
        }
    }
}