using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BuildIt.CognitiveServices;
using CognitiveServicesDemo.Common;
using CognitiveServicesDemo.Model;
using MvvmCross.Core.ViewModels;
using Newtonsoft.Json;
using System.IO;

namespace CognitiveServicesDemo.ViewModels
{
    public class SearchBingAutosuggestViewModel : MvxViewModel
    {
        private string inputText = PreInputText.BingAutosuggestText;
        private string outputText;
        private List<string> resultUrl;
        private List<string> resultName;

        public ObservableCollection<SearchSuggestion> BingAutoSuggest { get; set; } = new ObservableCollection<SearchSuggestion>();

        public string InputText
        {
            get { return inputText; }
            set
            {
                inputText = value;
                RaisePropertyChanged(() => InputText);
            }
        }

        public List<string> ResultUrl
        {
            get { return resultUrl; }
            set
            {
                resultUrl = value; 
                RaisePropertyChanged(() => ResultUrl);
            }
        }

        public List<string> ResultName
        {
            get { return resultName; }
            set
            {
                resultName = value; 
                RaisePropertyChanged(() => ResultName);
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

        public async Task BingAutoSuggestRequestAsync(string context)
        {
            try
            {
                var autosuggest = new AutosuggestAPIV5();
                var result = await autosuggest.SuggestionsWithHttpMessagesAsync(InputText, null, Constants.BingAutosuggestKey);
                var stream = await result.Response.Content.ReadAsStreamAsync();
                var serializer = new JsonSerializer();
                BingAutoSuggestApi feed;
                using (var sr = new StreamReader(stream))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    feed = serializer.Deserialize<BingAutoSuggestApi>(jsonTextReader);
                }
                //var cognitiveService = new CognitiveServiceClient();
                //var result =  await cognitiveService.BingAutosuggestApiRequestAsync(Constants.BingAutosuggestKey, InputText, System.Globalization.CultureInfo.CurrentUICulture.Name);

                var client = new HttpClient();

                /*
                //request header
                client.DefaultRequestHeaders.Add(Constants.SubscriptionTitle, Constants.BingAutosuggestKey);
                var queryString = $"q={context}";
                var body = string.Empty;
                var uri = "https://api.cognitive.microsoft.com/bing/v5.0/suggestions/?" + queryString;

                var response = await client.GetAsync(uri);
                var jsonResult = await response.Content.ReadAsStringAsync();
                //spellingCheckedText = jsonResult;
                var feed = JsonConvert.DeserializeObject<BingAutoSuggestApi>(jsonResult);
                */
                if (string.Equals(feed.statusCode, 200) || string.Equals(feed.statusCode, 0))
                {
                    foreach (var suggestion in feed.suggestionGroups[0].searchSuggestions)
                    {
                        BingAutoSuggest.Add(suggestion);
                    }
                }
                else
                {
                    OutputText= feed.message;
                }

                //AnalysisBreakIntoWord(feed);
                //await MakeSpellCheckRequestAsync(OutputText);
            }
            catch (Exception ex)
            {
                // ignored
            }
        }
    }
}
