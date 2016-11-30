using System;
using System.Net.Http;
using System.Threading.Tasks;
using BuildIt.CognitiveServices.Common;
using BuildIt.CognitiveServices.Models;
using BuildIt.CognitiveServices.Models.Feeds.Academic;
using BuildIt.CognitiveServices.Models.Feeds.InputParameters;
using BuildIt.CognitiveServices.Models.Feeds.Search;
using Newtonsoft.Json;

namespace BuildIt.CognitiveServices
{
    public class CognitiveServiceSearch
    {
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
                Console.WriteLine($"{ex}");
                
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
                Console.WriteLine($"{ex}");
            }
            return resultDto;
        }
    }
}
