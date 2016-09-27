using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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
        private VoiceCommandServiceConnection voiceServiceConnection;
        private BackgroundTaskDeferral serviceDeferral;

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
                await Task.Delay(1000);
                await ShowProgressScreen();
            }
            catch
            {
                Debug.WriteLine("Unable to process voice command");
            }
            serviceDeferral.Complete();
        }

        private async Task CortanaHelpList()
        {
            //back for cortana to show the content
            var msgback = new VoiceCommandUserMessage();
            msgback.DisplayMessage = msgback.SpokenMessage = "Here is the help list for you";
            //Cortana 
            var msgRepeat = new VoiceCommandUserMessage();
            msgRepeat.DisplayMessage = msgRepeat.SpokenMessage = "Here is another help list for you";

            var userMessage = new VoiceCommandUserMessage();
            userMessage.DisplayMessage = "Here is the help list for you";
            userMessage.SpokenMessage = "Here is the help list for you";

            var storageFile = await Package.Current.InstalledLocation.GetFileAsync("assets\\artwork.png");

            await ShowProgressScreen();

            //load temporary xml file
            var tempVoiceFile = await ApplicationData.Current.TemporaryFolder.GetFileAsync("_voices.xml");
            var randomAccessStream = await tempVoiceFile.OpenReadAsync();
            var stream = randomAccessStream.AsStreamForRead();

            var xml = XDocument.Load(stream);

            // Create items for each item. Ideally, should be limited to a small number of items.
            var destinationContentTiles = new List<VoiceCommandContentTile>();
            var ns = XNamespace.Get("http://schemas.microsoft.com/voicecommands/1.2");
            var xmlns = XNamespace.Get("http://www.w3.org/XML/1998/namespace");
            //get current user location
            var currentLocation = CultureInfo.CurrentCulture.Name.ToLower();
            //get CommandSet which match currentLocation
            var commandSet = (from c in xml.Descendants()
                              where ns.GetName("CommandSet") == c.Name
                              where c.Attribute(xmlns.GetName("lang")).Value == currentLocation
                              select c);
            //get all command in a list
            var commandList = (from c in commandSet.Descendants()
                               where ns.GetName("Command") == c.Name
                               select c).ToList();
            var totalCommandNo = Math.Min(commandList.Count, 4);

            // var test = new VoiceCommandContentTile();


            foreach (var command in commandList.Take(totalCommandNo))
            {
                destinationContentTiles.Add(new VoiceCommandContentTile
                {
                    AppLaunchArgument = command.Attribute("Name").Value,
                    ContentTileType = VoiceCommandContentTileType.TitleOnly,
                    Title = command.Element(ns.GetName("Example")).Value,
                });
            }
            if (totalCommandNo == 4)
            {
                var nextPage = new VoiceCommandContentTile
                {
                    ContentTileType = VoiceCommandContentTileType.TitleOnly,
                    Title = "More voice commands",
                    AppLaunchArgument = "More",
                };

                destinationContentTiles.Add(nextPage);
            }

            TilesList:

            // Cortana will handle re-prompting if the user does not provide a valid response.
            var response = VoiceCommandResponse.CreateResponseForPrompt(msgback, msgRepeat, destinationContentTiles);
            // If cortana is dismissed in this operation, null will be returned.

            var selectedRes = await voiceServiceConnection.RequestDisambiguationAsync(response);

            //Create dialogue confirm that user selected
            msgback.DisplayMessage = msgback.SpokenMessage = "Are you sure you want select " + selectedRes.SelectedItem.Title + " ?";
            msgRepeat.DisplayMessage = msgRepeat.SpokenMessage = "Please select Yes or No";
            response = VoiceCommandResponse.CreateResponseForPrompt(msgback, msgRepeat);

            //var voiceAppLaunchArgument = string.Empty;
            //return YES OR NO

            var result = await voiceServiceConnection.RequestConfirmationAsync(response);
            if (result.Confirmed)
            {
                var testTilesList = new List<VoiceCommandContentTile>();
                if (selectedRes.SelectedItem.AppLaunchArgument == "More")
                {
                    for (int i = totalCommandNo; i < commandList.Count - 1; i++)
                    {
                        testTilesList.Add(new VoiceCommandContentTile
                        {

                            AppLaunchArgument = commandList[i].Attribute("Name").Value,
                            ContentTileType = VoiceCommandContentTileType.TitleOnly,
                            Title = commandList[i].Element(ns.GetName("Example")).Value,

                        });
                    }
                    await
                voiceServiceConnection.ReportSuccessAsync(VoiceCommandResponse.CreateResponse(userMessage,
                    testTilesList));
                    return;
                }
                msgback.DisplayMessage = msgback.SpokenMessage = $"You've selected {selectedRes.SelectedItem.Title}";
                msgRepeat.DisplayMessage = msgRepeat.SpokenMessage = $"You've selected {selectedRes.SelectedItem.Title}";
                response = VoiceCommandResponse.CreateResponseForPrompt(msgback, msgRepeat);
                //var value = selectedRes.SelectedItem.AppLaunchArgument;
                //voiceAppLaunchArgument = selectedRes.SelectedItem.AppLaunchArgument;
                //response = VoiceCommandResponse.CreateResponse()
            }
            else
            {
                goto TilesList;
            }

            await voiceServiceConnection.ReportSuccessAsync(response);
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
            var userMessage = new VoiceCommandUserMessage { SpokenMessage = "Please launch app first", DisplayMessage = "Please launch app first" };

            var response = VoiceCommandResponse.CreateResponse(userMessage);

            await voiceServiceConnection.RequestAppLaunchAsync(response);
        }

        private void OnVoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
        {
            serviceDeferral?.Complete();
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            System.Diagnostics.Debug.WriteLine("Task cancelled, clean up");
            // Complete the service deferral
            serviceDeferral?.Complete();
        }
    }
}
