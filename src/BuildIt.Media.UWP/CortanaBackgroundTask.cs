using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;

namespace BuildIt.Media
{
    public class CortanaBackgroundTask : IBackgroundTask
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
                if (voiceCommand.CommandName == "help")
                {
                    var props = voiceCommand.Properties;

                    var searchTerm = default(string);

                    foreach (var kvp in props)
                    {
                        Debug.WriteLine($"{kvp.Key} - {string.Join(",", kvp.Value)}");
                        if (kvp.Key == "searchterm")
                        {
                            searchTerm = kvp.Value.FirstOrDefault();
                        }
                    }

                    if (string.IsNullOrEmpty(searchTerm) || searchTerm == "I" || string.IsNullOrWhiteSpace(searchTerm.Trim('.')))
                    {
                        await LaunchAppInForeground();
                    }
                    else
                    {
                        await CortanaHelpList(searchTerm);
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Unable to process voice command");
            }
        }

        private async Task CortanaHelpList(string searchTerm)
        {
            await ShowProgressScreen();
            var userMessage = new VoiceCommandUserMessage();
            var destinationContentTiles = new List<VoiceCommandContentTile>();
            userMessage.DisplayMessage = "Here is the help list for you";
            userMessage.SpokenMessage = "Here is the help list for you";

            var storageFile = await Package.Current.InstalledLocation.GetFileAsync("assets\\artwork.png");

            var play = new VoiceCommandContentTile
            {
                ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                Title = "play",
                Image = storageFile,
            };
            var pause = new VoiceCommandContentTile
            {
                ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                Title = "pause",
                Image = storageFile
            };

            destinationContentTiles.Add(pause);
            destinationContentTiles.Add(play);

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
            var userMessage = new VoiceCommandUserMessage { SpokenMessage = "Please launch app first" };

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