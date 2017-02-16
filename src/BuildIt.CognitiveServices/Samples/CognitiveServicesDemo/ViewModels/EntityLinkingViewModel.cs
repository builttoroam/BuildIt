using BuildIt.CognitiveServices;
using CognitiveServicesDemo.Common;
using CognitiveServicesDemo.Model;
using MvvmCross.Core.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
                RaisePropertyChanged(() => InputText);
            }
        }

        public string OutPutText
        {
            get { return outPut; }
            set
            {
                outPut = value;
                RaisePropertyChanged(() => OutPutText);
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


        public async Task EntityLinkingRequestAsync(string context)
        {
            try
            {
                EntityLinking feed;
                using (var entityLinking = new EntityLinkingAPI())
                {
                    feed = await entityLinking.Request<EntityLinkingAPI, EntityLinking>(
                        client =>
                            client.LinkEntityWithHttpMessagesAsync(InputText, null, null, Constants.EntityLinkingKey));
                }
                /*
                //var cognitiveService = new CognitiveServiceClient();
                //var result = await cognitiveService.EntityLinkingApiRequestAsync(Constants.EntityLinkingKey, context);

                var client = new HttpClient();
                
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
                */
                var count = 0;
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
