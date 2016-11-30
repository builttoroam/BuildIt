﻿using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using BuildIt.CognitiveServices;
using BuildIt.CognitiveServices.Models.Feeds.InputParameters;
using MvvmCross.Core.ViewModels;
using CognitiveServicesDemo.Common;
using Newtonsoft.Json;
using CognitiveServicesDemo.Model;

namespace CognitiveServicesDemo.ViewModels
{
    public class AcademicViewModel : MvxViewModel
    {
        private string warningText;
        private string inputText = "visual stu";
        private string outputText;

        public ObservableCollection<Interpretation> InterpretationParse { get; set; } = new ObservableCollection<Interpretation>();

        public string WarningText
        {
            get { return warningText; }
            set
            {
                warningText = value;
                RaisePropertyChanged(() => WarningText);
            }
        }

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

        public async Task AcademicSearchRequestAsync(string context)
        {
            try
            {
                WarningText = String.Empty;
                var client = new HttpClient();

                CognitiveServiceClient co = new CognitiveServiceClient();
                //CognitiveServiceKnowledge co = new CognitiveServiceKnowledge();
                await co.AcademicInterpretApiRequestAsync(new AcademicParameters()
                {
                    subscriptionKey = Constants.AcademicKey,
                    content = InputText
                });
                //request header
                client.DefaultRequestHeaders.Add(Constants.SubscriptionTitle, Constants.AcademicKey);
                //var queryString = $"q={context}&count=10&offset=0&mkt=en-us&safesearch=Moderate";
                var queryString = $"query={InputText}&complete=1&model=latest";
                var uri = "https://api.projectoxford.ai/academic/v1.0/interpret?" + queryString;

                var response = await client.GetAsync(uri);
                var jsonResult = await response.Content.ReadAsStringAsync();
                //spellingCheckedText = jsonResult;
                var feed = JsonConvert.DeserializeObject<AcademicFeeds>(jsonResult);
                InterpretationParse.Clear();
                if (feed.error !=null)
                {
                    WarningText = feed.error.message;
                }
                else
                {
                    foreach (var interp in feed.interpretations)
                    {
                        InterpretationParse.Add(interp);
                    }
                }

            }
            catch (Exception ex)
            {
                // ignored
            }
        }
    }


}
