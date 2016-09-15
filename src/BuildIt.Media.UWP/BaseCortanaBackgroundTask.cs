using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;

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
            var destinationContentTiles = new List<VoiceCommandContentTile>();
            userMessage.DisplayMessage = "Here is the help list for you";
            userMessage.SpokenMessage = "Here is the help list for you";

            var storageFile = await Package.Current.InstalledLocation.GetFileAsync("assets\\artwork.png");

            var play = new VoiceCommandContentTile
            {
                ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                Title = "play",
                Image = storageFile,
                AppLaunchArgument = "buildit_play"
        };
            var pause = new VoiceCommandContentTile
            {
                ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                Title = "pause",
                Image = storageFile,
                AppLaunchArgument = "buildit_pause"
            };
            var forward = new VoiceCommandContentTile
            {
                ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                Title = "forward",
                Image = storageFile,
                AppLaunchArgument = "buildit_forward"
            };
            var back = new VoiceCommandContentTile
            {
                ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                Title = "back",
                Image = storageFile,
                AppLaunchArgument = "buildit_back"
            };
            var volumeup = new VoiceCommandContentTile
            {
                ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                Title = "volumeup",
                Image = storageFile,
                AppLaunchArgument = "buildit_volumeup"
            };
            var volumedown = new VoiceCommandContentTile
            {
                ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                Title = "volumedown",
                Image = storageFile,
                AppLaunchArgument = "buildit_volumedown"
            };
            var mute = new VoiceCommandContentTile
            {
                ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                Title = "mute",
                Image = storageFile,
                AppLaunchArgument = "buildit_mute"
            };
            var unmute = new VoiceCommandContentTile
            {
                ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                Title = "unmute",
                Image = storageFile,
                AppLaunchArgument = "buildit_unmute"
            };

            destinationContentTiles.Add(pause);
            destinationContentTiles.Add(play);
            destinationContentTiles.Add(forward);
            destinationContentTiles.Add(back);
            destinationContentTiles.Add(volumeup);
            destinationContentTiles.Add(volumedown);
            destinationContentTiles.Add(mute);
            destinationContentTiles.Add(unmute);

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
