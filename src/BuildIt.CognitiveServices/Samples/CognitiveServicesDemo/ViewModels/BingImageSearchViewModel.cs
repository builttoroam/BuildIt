using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using BuildIt.CognitiveServices;
using CognitiveServicesDemo.Common;
using CognitiveServicesDemo.Model;
using MvvmCross.Core.ViewModels;
using Newtonsoft.Json;
using System.IO;

namespace CognitiveServicesDemo.ViewModels
{
    public class BingImageSearchViewModel : MvxViewModel
    {
        private string inputText = "bill gates";
        private string outputText;

        public ObservableCollection<BingImageSearchThumbnail2> ImageSearchResults { get; set; } = new ObservableCollection<BingImageSearchThumbnail2>();

        public string InputText
        {
            get { return inputText; }
            set
            {
                inputText = value;
                RaisePropertyChanged(() => InputText);
            }
        }

        public string OutputText
        {
            get { return outputText; }
            set
            {
                outputText = value;
                RaisePropertyChanged(() => OutputText);
            }
        }


        public async Task BingSearchRequestAsync(string context)
        {
            try
            {
                ImageSearchAPIV5 imageSearch = new ImageSearchAPIV5();
                var result = await imageSearch.SearchWithHttpMessagesAsync(InputText,null, 1, null, null, null, Constants.BingSearchKey);
                var stream = await result.Response.Content.ReadAsStreamAsync();
                var serializer = new JsonSerializer();
                BingImageSearchFeeds feed;
                using (var sr = new StreamReader(stream))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    feed = serializer.Deserialize<BingImageSearchFeeds>(jsonTextReader);
                }
                /*
                var client = new HttpClient();
                //request header
                client.DefaultRequestHeaders.Add(Constants.SubscriptionTitle, Constants.BingSearchKey);
                var queryString = $"q={context}&count=10&offset=0&mkt=en-us&safesearch=Moderate";
                var uri = "https://api.cognitive.microsoft.com/bing/v5.0/search?" + queryString;

                var response = await client.GetAsync(uri);
                var jsonResult = await response.Content.ReadAsStringAsync();
                //spellingCheckedText = jsonResult;
                var feed = JsonConvert.DeserializeObject<BingWebSearch>(jsonResult);
                ImageSearchResults.Clear();
                */
                if (feed != null)
                {
                    foreach (var value in feed.queryExpansions)
                    {
                        ImageSearchResults.Add(value.thumbnail);
                    }

                }
                else
                {
                    OutputText = "Error";
                }

            }
            catch (Exception ex)
            {
                // ignored
            }
        }
    }
}
