using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using CognitiveServicesDemo.Common;
using CognitiveServicesDemo.Model;
using Newtonsoft.Json;
using BuildIt.CognitiveServices;
using Xamarin.Forms;

namespace CognitiveServicesDemo.ViewModels
{
    public class EntityLinkingViewModel : MvxViewModel
    {
        
        private string input = PreInputText.EntityLinkingText;
        private string outPut;
        private string warningText;


        public ObservableCollection<Entity> EntityLinkings { get; set; } = new ObservableCollection<Entity>();


        public string InputText
        {
            get { return input; }
            set
            {
                input = value;
                RaisePropertyChanged(()=>InputText);
            }
        }

        public string OutPutText
        {
            get { return outPut; }
            set
            {
                outPut = value; 
                RaisePropertyChanged(() =>OutPutText);
            }
        }

        public string WarningText
        {
            get { return warningText; }
            set
            {
                warningText = value; 
                RaisePropertyChanged(() =>WarningText);
            }
        }


        public async Task EntityLinkingRequestAsync(string context)
        {
            try
            {
                var cognitiveService = new CognitiveServiceClient();
                var result = await cognitiveService.EntityLInkingApiRequestAsync(Constants.EntityLinkingKey, context);



                
                var client = new HttpClient();
                var count = 0;
                //request header
                client.DefaultRequestHeaders.Add(Constants.SubscriptionTitle, Constants.EntityLinkingKey);
                var body = context;
                var uri = "https://api.projectoxford.ai/entitylinking/v1.0/link";
                HttpResponseMessage response;
                //request body
                byte[] byteData = Encoding.UTF8.GetBytes(body);

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                    response = await client.PostAsync(uri, content);
                }

                var jsonResult = await response.Content.ReadAsStringAsync();
                
                var feed = JsonConvert.DeserializeObject<EntityLinking>(jsonResult);
                
                foreach (var entity in feed.entities)
                {
                    EntityLinkings.Add(entity);
                }
                //EntityLinkings = new ObservableCollection<EntityLinking>();

                OutPutText = null;
                //if (!string.Equals(Linking.code,"200") || !string.Equals(Linking.code,"0") || !string.IsNullOrEmpty(Linking.code))
                //{
                //    foreach (var feeds in Linking.entities)
                //    {


                //        foreach (var match in feeds.matches)
                //        {
                //            count++;
                //            OutPutText += $"Word{count}: Name: {match.text}, wikipediaId: {feeds.wikipediaId}. ";
                //        }
                //    }
                    
                //}
                //else
                //{
                //    OutPutText = Linking.message;
                //}
                if (feed.entities != null)
                {
                    foreach (var feeds in feed.entities)
                    {


                        foreach (var match in feeds.matches)
                        {
                            count++;
                            OutPutText += $"Word{count}: Name: {match.text}, wikipediaId: {feeds.wikipediaId}. ";
                        }
                    }
                }
                else
                {
                    OutPutText = feed.message;
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
