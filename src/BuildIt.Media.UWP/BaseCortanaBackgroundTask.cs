﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;

namespace BuildIt.Media
{
    public class BaseCortanaBackgroundTask
    {

        VoiceCommandServiceConnection voiceServiceConnection;
        BackgroundTaskDeferral serviceDeferral;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            serviceDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnTaskCanceled;

            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            if (triggerDetails == null)
            {
                serviceDeferral.Complete();
                return;
            }

            try
            {
                voiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);
                voiceServiceConnection.VoiceCommandCompleted += OnVoiceCommandCompleted;

                var voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();

                //Find the command name witch should match the VCD
                if (voiceCommand.CommandName == "buildit_help")
                {
                    var props = voiceCommand.Properties;
                    await CortanaHelpList();
                }
            }
            catch
            {
                Debug.WriteLine("Unable to process voice command");
            }
            serviceDeferral.Complete();
        }

        private async Task CortanaHelpList()
        {
            await ShowProgressScreen();
            var userMessage = new VoiceCommandUserMessage();
            userMessage.DisplayMessage = "Here is the help list for you";
            userMessage.SpokenMessage = "Here is the help list for you";

            var storageFile = await Package.Current.InstalledLocation.GetFileAsync("assets\\artwork.png");

            //load temporary xml file
            var tempVoiceFile = await ApplicationData.Current.TemporaryFolder.GetFileAsync("_voices.xml");
            var randomAccessStream = await tempVoiceFile.OpenReadAsync();
            var stream = randomAccessStream.AsStreamForRead();

            var xml = XDocument.Load(stream);

            var ns = XNamespace.Get("http://schemas.microsoft.com/voicecommands/1.2");
            var xmlns = XNamespace.Get("http://www.w3.org/XML/1998/namespace");
            var commandName = (from c in xml.Descendants()
                where ns.GetName("Command") == c.Name
                select c).ToList();
            //var commandTitle = (from c in xml.Declaration)

            var destinationContentTiles = commandName.Take(4).Select(command => new VoiceCommandContentTile
            {
                ContentTileType = VoiceCommandContentTileType.TitleWith68x92IconAndText,
                AppLaunchArgument = command.Attribute("Name").Value,
                Title = command.Element(ns.GetName("Example")).Value,
                Image = storageFile
            }).ToList();

            //var play = new VoiceCommandContentTile
            //{
            //    ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
            //    Title = "play",
            //    Image = storageFile,
            //    AppLaunchArgument = "buildit_play"
            //};
            //var pause = new VoiceCommandContentTile
            //{
            //    ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
            //    Title = "pause",
            //    Image = storageFile,
            //    AppLaunchArgument = "buildit_pause"
            //};
            //var forward = new VoiceCommandContentTile
            //{
            //    ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
            //    Title = "forward",
            //    Image = storageFile,
            //    AppLaunchArgument = "buildit_forward"
            //};
            //var back = new VoiceCommandContentTile
            //{
            //    ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
            //    Title = "back",
            //    Image = storageFile,
            //    AppLaunchArgument = "buildit_back"
            //};

            
            //destinationContentTiles.Add(pause);
            //destinationContentTiles.Add(play);
            //destinationContentTiles.Add(forward);
            //destinationContentTiles.Add(back);
            //destinationContentTiles.Add(volumeup);
            //destinationContentTiles.Add(volumedown);
            //destinationContentTiles.Add(mute);
            //destinationContentTiles.Add(unmute);

            await
                voiceServiceConnection.ReportSuccessAsync(VoiceCommandResponse.CreateResponse(userMessage,
                    destinationContentTiles));
        }

        private async Task ShowProgressScreen()
        {
            var userProgressMessage = new VoiceCommandUserMessage();
            userProgressMessage.DisplayMessage = userProgressMessage.SpokenMessage =
                "Searching voice commands....";

            var response = VoiceCommandResponse.CreateResponse(userProgressMessage);
            await voiceServiceConnection.ReportProgressAsync(response);
        }

        private async Task LaunchAppInForeground()
        {
            var userMessage = new VoiceCommandUserMessage { SpokenMessage = "Please launch app first",DisplayMessage = "Please launch app first" };

            var response = VoiceCommandResponse.CreateResponse(userMessage);

            await voiceServiceConnection.RequestAppLaunchAsync(response);
        }

        private void OnVoiceCommandCompleted(
    VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
        {
            if (this.serviceDeferral != null)
            {
                this.serviceDeferral.Complete();
            }
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            System.Diagnostics.Debug.WriteLine("Task cancelled, clean up");
            if (this.serviceDeferral != null)
            {
                // Complete the service deferral
                this.serviceDeferral.Complete();
            }
        }
    }
}
