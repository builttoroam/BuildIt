using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BuildIt.CognitiveServices;
using BuildIt.CognitiveServices.Models.Feeds.InputParameters;
using BuildIt.CognitiveServices.Models;
using CognitiveServicesDemo.Model;
using MvvmCross.Core.ViewModels;
using Newtonsoft.Json;
using CognitiveServicesDemo.Common;

namespace CognitiveServicesDemo.ViewModels
{
    public class LanguageTextAnalyticsViewModel : MvxViewModel
    {
        private string inputText = "I had a wonderful experience! The rooms were wonderful and the staff were helpful.";
        private string warningText;
        private string language;
        private string isoName = "en";
        private string keyPhrases;
        private string sentimentResult;

        public string InputText
        {
            get { return inputText; }
            set
            {
                inputText = value;
                RaisePropertyChanged(() => InputText);
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

        public string IsoName
        {
            get { return isoName; }
            set
            {
                isoName = value;
                RaisePropertyChanged(() => IsoName);
            }
        }

        public string Language
        {
            get { return language; }
            set
            {
                language = value;
                RaisePropertyChanged(() => Language);
            }
        }

        public string KeyPhrases
        {
            get { return keyPhrases; }
            set
            {
                keyPhrases = value;
                RaisePropertyChanged(() => KeyPhrases);
            }
        }

        public string SentimentResult
        {
            get { return sentimentResult; }
            set
            {
                sentimentResult = value;
                RaisePropertyChanged(() => SentimentResult);
            }
        }


        public async Task TextAnalyticsAsync(string inputText)
        {
            //TextAnalyticsReplyBody text = new TextAnalyticsReplyBody();
            //text.documents[0].id = DateTime.Now.ToString();
            //text.documents[0].
            //text.text = inputText;
            //text.id = DateTime.Now.ToString();
            //var jsonString = JsonConvert.SerializeObject(text);
            var detectLanguage = " {\"documents\": [{ \"id\":\"" + DateTime.Now + "\", \"text\":\"" + inputText + "\"}]}";

            //HTTPClient
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Constants.TextAnalyticsKey);

            //request parameters
            var uri = $"https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/languages?";

            HttpResponseMessage response;

            //var cognitiveService = new CognitiveServiceClient();
            //var result = await cognitiveService.DetectLanguageApiRequestAsync(Constants.TextAnalyticsKey, detectLanguage);



            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(detectLanguage);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }
            var jsonResult = await response.Content.ReadAsStringAsync();
            var feed = JsonConvert.DeserializeObject<TextAnalyticsReplyBody>(jsonResult);
            


            //*****************************************
            //continue call keyPhrases API
            var detectkeyPhrases = " {\"documents\": [{ \"id\":\"" + DateTime.Now + "\", \"text\":\"" + inputText + "\", \"language\": \"" + IsoName + "\" }]}";
            var keyPhrasesUri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/keyPhrases?";

            HttpResponseMessage response2;

            //var cognitiveService = new CognitiveServiceClient();
            //var result = await cognitiveService.KeyPhrasesApiRequestAsync(Constants.TextAnalyticsKey, detectkeyPhrases);

            // Request body
            byte[] byteData2 = Encoding.UTF8.GetBytes(detectkeyPhrases);

            using (var content = new ByteArrayContent(byteData2))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response2 = await client.PostAsync(keyPhrasesUri, content);
            }
            var jsonResult2 = await response2.Content.ReadAsStringAsync();
            var feed2 = JsonConvert.DeserializeObject<DetectkeyPhrases>(jsonResult2);
            



            //******************************
            //continue call sentiment API
            var detectSentiment = " {\"documents\": [{ \"id\":\"" + DateTime.Now + "\", \"text\":\"" + inputText + "\", \"language\": \"" + IsoName + "\" }]}";
            var SentimentUri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment?";

            HttpResponseMessage response3;

            var cognitiveService = new CognitiveServiceClient();
            var sentiment = await cognitiveService.SentimentApiRequestAsync(Constants.TextAnalyticsKey, detectSentiment);
            
            // Request body
            byte[] byteData3 = Encoding.UTF8.GetBytes(detectSentiment);

            using (var content = new ByteArrayContent(byteData3))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response3 = await client.PostAsync(SentimentUri, content);
            }
            var jsonResult3 = await response3.Content.ReadAsStringAsync();
            var feed3 = JsonConvert.DeserializeObject<Sentiment>(jsonResult3);


            AnalysisResult(feed);
            AnalysisKeyPhraseResult(feed2);
            AnalysisSentimentResult(feed3);

        }

        private void AnalysisResult(TextAnalyticsReplyBody feed)
        {
            foreach (var f in feed.documents)
            {
                foreach (var l in f.detectedLanguages)
                {
                    Language = $"{l.name}(confidence:{l.score:p})";
                    IsoName = l.iso6391Name;
                }
            }
            if (string.Equals(IsoName, "en") || string.Equals(IsoName, "ja") || string.Equals(IsoName, "de") || string.Equals(IsoName, "es"))
            {
                WarningText = "";
            }
            else
            {
                WarningText = $"Key phases and sentiment score cannot be detected from {Language} text at this time.";
                KeyPhrases = "";
                SentimentResult = "";
            }
        }

        private void AnalysisKeyPhraseResult(DetectkeyPhrases feed)
        {
            foreach (var doc in feed.documents)
            {
                foreach (var keys in doc.keyPhrases)
                {
                    KeyPhrases += $"{keys}, ";
                }

            }
        }

        private void AnalysisSentimentResult(Sentiment feed)
        {
            foreach (var f in feed.documents)
            {
                SentimentResult = $"{f.score:p}";
            }
        }
    }
}
