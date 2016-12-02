using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using CognitiveServicesDemo.Common;
using CognitiveServicesDemo.Model;
using MvvmCross.Core.ViewModels;
using Newtonsoft.Json;
using BuildIt.CognitiveServices;
using BuildIt.CognitiveServices.Models.Feeds.InputParameters;

namespace CognitiveServicesDemo.ViewModels
{
    public class BingWebSearchViewModel : MvxViewModel
    {
        private string inputText = "bill gates";
        private string outputText;

        public ObservableCollection<Value> WebSearchResults { get; set; } = new ObservableCollection<Value>();

        public string InputText
        {
            get { return inputText; }
            set
            {
                inputText = value;
                RaisePropertyChanged(() =>InputText);
            }
        }

        public string OutputText
        {
            get { return outputText; }
            set
            {
                outputText = value; 
                RaisePropertyChanged(()=> OutputText);
            }
        }


        public async Task BingSearchRequestAsync(string context)
        {
            try
            {
                var co = new CognitiveServiceClient();
                var result = await co.BingSearchApiRequestAsync(new BingSearchParameters()
                {
                    content = InputText,
                    subscriptionKey = Constants.BingSearchKey
                });

                var client = new HttpClient();

                //request header
                client.DefaultRequestHeaders.Add(Constants.SubscriptionTitle, Constants.BingSearchKey);
                var queryString = $"q={context}&count=10&offset=0&mkt=en-us&safesearch=Moderate";
                var uri = "https://api.cognitive.microsoft.com/bing/v5.0/search?" + queryString;

                var response = await client.GetAsync(uri);
                var jsonResult = await response.Content.ReadAsStringAsync();
                //spellingCheckedText = jsonResult;
                var feed = JsonConvert.DeserializeObject<BingWebSearch>(jsonResult);
                WebSearchResults.Clear();
                if (string.Equals(feed.statusCode, 200) || string.Equals(feed.statusCode, 0))
                {
                    foreach (var value in feed.webPages.value)
                    {
                        WebSearchResults.Add(value);
                    }
                    
                }
                else
                {
                    OutputText = feed.message;
                }
                
            }
            catch (Exception ex)
            {
            }
        }
    }
}
