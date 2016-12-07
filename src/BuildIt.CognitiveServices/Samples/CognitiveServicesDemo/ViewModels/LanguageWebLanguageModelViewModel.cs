using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BuildIt.CognitiveServices;
using BuildIt.CognitiveServices.Models.Feeds.InputParameters;
using BuildIt.CognitiveServices.Models;
using CognitiveServicesDemo.Common;
using CognitiveServicesDemo.Model;
using MvvmCross.Core.ViewModels;
using Newtonsoft.Json;

namespace CognitiveServicesDemo.ViewModels
{
    public class LanguageWebLanguageModelViewModel : MvxViewModel
    {
        private string inputText = "Tryoutwordbreakingbytypingasentence";
        private string outputText;
        private string spellCheckedText;

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
                RaisePropertyChanged(() =>OutputText);
            }
        }

        public string SpellCheckedText
        {
            get { return spellCheckedText; }
            set
            {
                spellCheckedText = value; 
                RaisePropertyChanged(() => SpellCheckedText);
            }
        }


        public async Task BreakIntoWordRequestAsync(string context)
        {
            try
            {
                var client = new HttpClient();

                //var co = new CognitiveServiceClient();
                //var result = await co.BreakIntoWordsApiRequestAsync(new BreakIntoWordsParameters()
                //{
                //    subscriptionKey = Constants.WebLanguageModelKey,text = InputText
                //});

                WebLanguageModelAPI web = new WebLanguageModelAPI();
                var re = await web.BreakIntoWordsWithHttpMessagesAsync("title",
                    "Tryoutwordbreakingbytypingasetenceorclickingthesamplesbelow",5, 5, null, "f13480095cdd4c8aad2115993f668a20");
                var stream = await re.Response.Content.ReadAsStreamAsync();
                var serializer = new JsonSerializer();
                WebLanguageModel result;
                using (var sr = new StreamReader(stream))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    result = serializer.Deserialize<WebLanguageModel>(jsonTextReader);
                }
                
                /* Old api call method
                //request header
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "f13480095cdd4c8aad2115993f668a20");
                var queryString = $"model=title&text={context}&order=5&maxNumOfCandidatesReturned=5";
                var body = string.Empty;
                var uri = "https://api.projectoxford.ai/text/weblm/v1.0/breakIntoWords?" + queryString;
                HttpResponseMessage response;
                //request body
                byte[] byteData = Encoding.UTF8.GetBytes(body);

                using (var content =new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    response = await client.PostAsync(uri, content);
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                var feed = JsonConvert.DeserializeObject<WebLanguageModel>(jsonResult);
                */

                AnalysisBreakIntoWord(result);
                await MakeSpellCheckRequestAsync(OutputText);
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        public static object DeserializeFromStream(Stream stream)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize(jsonTextReader);
            }
        }

        private async void AnalysisBreakIntoWord(WebLanguageModel feed)
        {
            var bestResult =0.0;

            for (int i = 0; i < feed.candidates.Count; i++)
            {
                if (i == 0)
                {
                    bestResult = feed.candidates[i].probability;
                    OutputText = feed.candidates[i].words;
                }
                if (!(feed.candidates[i].probability > bestResult)) continue;
                feed.candidates[i].probability = bestResult;
                //OutputText = feed.candidates[i].words;
                //OutputText = "Analysing now";
            }
        }

        public async Task MakeSpellCheckRequestAsync(string context)
        {
            try
            {
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
                var uri = "https://api.cognitive.microsoft.com/bing/v5.0/spellcheck/?" + queryString;

                var response = await client.GetAsync(uri);
                var jsonResult = await response.Content.ReadAsStringAsync();
                //spellingCheckedText = jsonResult;
                var feed = JsonConvert.DeserializeObject<BingSpellCheckFeed>(jsonResult);
                if (string.Equals(feed.statusCode, 200) || string.Equals(feed.statusCode, 0))
                {
                    ReplaceAfterSpellCheck(context, feed);
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

        private void ReplaceAfterSpellCheck(string inputContext, BingSpellCheckFeed feed)
        {
            var result = string.Empty;
            foreach (var f in feed.flaggedTokens)
            {
                result = inputContext.Replace(f.token, f.suggestions[0].suggestion);
            }
            //if (string.IsNullOrEmpty(result))
            //{
            //    SpellCheckedText =""
            //}
            SpellCheckedText = $"Spell checked: {result}";
        }
    }
}
