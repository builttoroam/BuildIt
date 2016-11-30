using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BuildIt.CognitiveServices.Common;
using BuildIt.CognitiveServices.Models;
using BuildIt.CognitiveServices.Models.Feeds.InputParameters;
using BuildIt.CognitiveServices.Models.Feeds.Language;
using Newtonsoft.Json;

namespace BuildIt.CognitiveServices
{
    public class CognitiveServiceLanguage
    {
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
                Console.WriteLine($"Error: {ex}");
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
                if (!response.IsSuccessStatusCode || response.Content == null) return null;
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
                Console.WriteLine($"Error: {ex}");
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
                if (!response.IsSuccessStatusCode || response.Content == null) return null;
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
                Console.WriteLine($"Error: {ex}");

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
                Console.WriteLine($"Error: {ex}");
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
                Console.WriteLine($"{ex}");
            }
            return resultDto;
        }
    }
}
