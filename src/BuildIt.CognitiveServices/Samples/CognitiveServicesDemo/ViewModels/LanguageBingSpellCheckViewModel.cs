using System;
using System.Net.Http;
using System.Threading.Tasks;
using BuildIt.CognitiveServices.Models.Feeds.InputParameters;
using BuildIt.CognitiveServices;
using CognitiveServicesDemo.Model;
using MvvmCross.Core.ViewModels;
using Newtonsoft.Json;
using CognitiveServicesDemo.Common;
using System.IO;

namespace CognitiveServicesDemo.ViewModels
{
    public class LanguageBingSpellCheckViewModel : MvxViewModel
    {
        private string inputText ="A new service from micros oft!";

        private string spellingCheckedText;
        private string warning;

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
            get { return spellingCheckedText; }
            set
            {
                spellingCheckedText = value;
                RaisePropertyChanged(() =>OutputText);
            }
        }

        public string Warning
        {
            get { return warning; }
            set
            {
                warning = value;
                RaisePropertyChanged(() =>Warning);
            }
        }

        public async Task MakeSpellCheckRequestAsync(string context)
        {
            try
            {
                var spellCheck = new SpellCheckAPIV5();
                var result = await spellCheck.SpellCheckWithHttpMessagesAsync(InputText,null, null, null, "en-us", null, Constants.SpellCheckKey);
                var stream = await result.Response.Content.ReadAsStreamAsync();
                var serializer = new JsonSerializer();
                BingSpellCheckFeed feed;
                using (var sr = new StreamReader(stream))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    feed = serializer.Deserialize<BingSpellCheckFeed>(jsonTextReader);
                }
                /*
                var client = new HttpClient();
                var queryContext = string.Empty;
                if (!string.IsNullOrEmpty(context))
                {
                    queryContext = context.Replace(" ", "+");
                }
                var postContextText = string.Empty;
                var preContextText = string.Empty;

                // Request headers
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "5e5b5fe407dd4e76b081b9b344abe3c4");

                // Request parameters
                var queryString = $"text={queryContext}&mode=spell&preContextText={preContextText}&postContextText={postContextText}&mkt=en-us";
                //var queryString = HttpUtility.ParseQueryString(string.Empty);
                //queryString["text"] = context;
                //queryString["mode"] = "spell";
                //queryString["preContextText"] = preContextText;
                //queryString["postContextText"] = postContextText;
                //queryString["mkt"] = "en-au";
                var uri = "https://api.cognitive.microsoft.com/bing/v5.0/spellcheck/?" + queryString;
                
                var cognitiveServiceLanguage = new CognitiveServiceClient();
                var result = await cognitiveServiceLanguage.SpellCheckApiRequestAsync(new SpellCheckParameters()
                {
                    content = queryContext,
                    subscriptionKey = Constants.SpellCheckKey
                });

                var response = await client.GetAsync(uri);
                var jsonResult = await response.Content.ReadAsStringAsync();
                ////spellingCheckedText = jsonResult;
                var feed = JsonConvert.DeserializeObject<BingSpellCheckFeed>(jsonResult);
                */

                if (string.Equals(feed.statusCode, 200) || string.Equals(feed.statusCode, 0))
                {
                    ReplaceAfterSpellCheck(context, feed);
                }
                else
                {
                    Warning = feed.message;
                }

            }
            catch (Exception ex)
            {
                //System.Console.WriteLine(ex);
                // ignored
            }
        }

        private void ReplaceAfterSpellCheck(string inputContext, BingSpellCheckFeed feed)
        {
            var result = string.Empty;
            foreach (var f in feed.flaggedTokens)
            {
                result = inputContext.Replace(f.token, f.suggestions[0].suggestion);
            }
            OutputText = result;
        }
    }
}
