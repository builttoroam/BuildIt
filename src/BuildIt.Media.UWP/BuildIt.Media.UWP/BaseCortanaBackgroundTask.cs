using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Calls;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;

namespace BuildIt.Media
{
    public class BaseCortanaBackgroundTask
    {
        private static class VoiceCommandSchema
        {
            public static XName CommandSet => VoiceCommandNameSpace.GetName("CommandSet");
            public static XName Command => VoiceCommandNameSpace.GetName("Command");

            public static XName Example => VoiceCommandNameSpace.GetName("Example");
            public static XName Lang => XmlNameSpace.GetName("lang");
        }


        private VoiceCommandServiceConnection voiceServiceConnection;
        private BackgroundTaskDeferral serviceDeferral;

        private const string CortanaReply = "Here is the help list for you";
        private const string CortanaSecondReply = "Please select one";
        private const string MoreVoiceCommands = "More voice commands";
        private static XNamespace VoiceCommandNameSpace { get; } = XNamespace.Get("http://schemas.microsoft.com/voicecommands/1.2");
        private static XNamespace XmlNameSpace { get; } = XNamespace.Get("http://www.w3.org/XML/1998/namespace");


        public async Task<bool> Run(IBackgroundTaskInstance taskInstance)
        {
            serviceDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnTaskCanceled;

            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            if (triggerDetails == null)
            {
                serviceDeferral.Complete();
                return false;
            }

            try
            {
                voiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);
                voiceServiceConnection.VoiceCommandCompleted += OnVoiceCommandCompleted;

                var voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();

                //Find the command name witch should match the VCD
                if (voiceCommand.CommandName == "buildit_help")
                {
                    //var props = voiceCommand.Properties;
                    await CortanaHelpList();
                    return true;
                }
                //await Task.Delay(1000);
                //await ShowProgressScreen();
                return false;
            }
            catch
            {
                Debug.WriteLine("Unable to process voice command");
                return false;
            }
            finally
            {
                serviceDeferral.Complete();
            }
            
        }

        private async Task CortanaHelpList()
        {
            //back for cortana to show the content
            var msgback = new VoiceCommandUserMessage();
            msgback.DisplayMessage = msgback.SpokenMessage = CortanaReply;
            //Cortana 
            var msgRepeat = new VoiceCommandUserMessage();
            msgRepeat.DisplayMessage = msgRepeat.SpokenMessage = CortanaSecondReply;

            var userMessage = new VoiceCommandUserMessage();
            userMessage.DisplayMessage = CortanaReply;
            userMessage.SpokenMessage = CortanaReply;
            var commandsTook = 0;
            var commandsCountingNo = 0;

            await ShowProgressScreen();

            //load temporary xml file
            var tempVoiceFile = await ApplicationData.Current.TemporaryFolder.GetFileAsync("Voices.xml");
            var randomAccessStream = await tempVoiceFile.OpenReadAsync();
            var stream = randomAccessStream.AsStreamForRead();

            var xml = XDocument.Load(stream);

            // Create items for each item. Ideally, should be limited to a small number of items.
            var destinationContentTiles = new List<VoiceCommandContentTile>();
            
            //get current user location
            var currentLocation = CultureInfo.CurrentCulture.Name.ToLower();
            //get CommandSet which match currentLocation
            var commandSet = (from c in xml.Descendants()
                              where VoiceCommandSchema.CommandSet == c.Name
                              where c.Attribute(VoiceCommandSchema.Lang).Value == currentLocation
                              select c);
            //get all command in a list
            var commandList = (from c in commandSet.Descendants()
                           where VoiceCommandSchema.Command == c.Name
                           select c).ToList();
            
            var cmtList = commandList.DescendantNodes().OfType<XComment>().ToList();

            var response = await CortanaList(destinationContentTiles, commandList, cmtList,commandsTook,commandsCountingNo);

            await voiceServiceConnection.ReportSuccessAsync(response);
        }

        private async Task<VoiceCommandResponse> CortanaList(List<VoiceCommandContentTile> destContentTiles, List<XElement> commandList, List<XComment> commentList ,int cmdsTook,int cmdCountingNo)
        {
            var destinationContentTiles = destContentTiles;
            var cmdList = commandList;
            var cmtList = commentList;
            var commandsTook = cmdsTook;
            var commandsCountingNo = cmdCountingNo;
            //back for cortana to show the content
            var msgback = new VoiceCommandUserMessage();
            msgback.DisplayMessage = msgback.SpokenMessage = CortanaReply;
            //Cortana 
            var msgRepeat = new VoiceCommandUserMessage();
            msgRepeat.DisplayMessage = msgRepeat.SpokenMessage = CortanaSecondReply;

            var moreCommands = "Select next page commands";

            var iconsFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("builditmedia");
            destinationContentTiles.Clear();
            if (cmdList.Count - commandsTook <= 5)
            {
                for (int i = commandsCountingNo; i <= cmdList.Count-1; i++)
                {
                    //var command = cmdList[commandsTook];
                    var attributeName = cmdList[commandsTook].Attribute("Name").Value;

                    var descriptionComment = (from comment in cmdList[commandsTook].DescendantNodes().OfType<XComment>()
                        where comment.Value.StartsWith("Description:")
                        select comment.Value)
                        .FirstOrDefault() // Get the first comment that starts with "Description:"
                        ?.Replace("Description:", "") ?? ""; // If one exists, trim "Description:" by replacing it with ""
                    if (attributeName.Contains("buildit") != true)
                    {
                        attributeName = "buildit_customTile";
                    }

                    //var iconFile = await iconsFolder.GetFileAsync($"{attributeName}.png");

                    destinationContentTiles.Add(new VoiceCommandContentTile
                    {
                        AppLaunchArgument = cmdList[commandsTook].Attribute("Name").Value,
                        ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                        Title = cmdList[commandsTook].Element(VoiceCommandSchema.Example).Value,
                        TextLine1 = descriptionComment,
                        Image = await iconsFolder.GetFileAsync($"{attributeName}.png")
                    });
                    commandsTook++;
                }
            }
            else
            {
                destinationContentTiles.Clear();
                for (int i = 0; i < 4; i++)
                {
                    var command = cmdList[commandsTook];
                    var attributeName = cmdList[commandsTook].Attribute("Name").Value;
                    var descriptionComment = (from comment in command.DescendantNodes().OfType<XComment>()
                        where comment.Value.StartsWith(" Description:")
                        select comment.Value)
                        .FirstOrDefault() // Get the first comment that starts with "Description:"
                        ?.Replace("Description:", "") ?? ""; // If one exists, trim "Description:" by replacing it with ""
                    if (attributeName.Contains("buildit") != true)
                    {
                        attributeName = "buildit_customTile";
                    }

                    //var iconFile = await iconsFolder.GetFileAsync($"{attributeName}.png");


                    destinationContentTiles.Add(new VoiceCommandContentTile
                    {
                        AppLaunchArgument = cmdList[commandsTook].Attribute("Name").Value,
                        ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                        Title = cmdList[commandsTook].Element(VoiceCommandSchema.Example).Value,
                        TextLine1 = descriptionComment,
                        Image = await iconsFolder.GetFileAsync($"{attributeName}.png")
                    });
                    commandsTook++;
                }
                var nextPage = new VoiceCommandContentTile
                {
                    ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                    Title = MoreVoiceCommands,
                    AppLaunchArgument = "more",
                    TextLine1 = moreCommands,
                    Image = await iconsFolder.GetFileAsync("buildit_help.png")
            };
                destinationContentTiles.Add(nextPage);

                commandsCountingNo += 4;
            }


            // Cortana will handle re-prompting if the user does not provide a valid response.
            var response = VoiceCommandResponse.CreateResponseForPrompt(msgback, msgRepeat, destinationContentTiles);
            // If cortana is dismissed in this operation, null will be returned.

            var selectedRes = await voiceServiceConnection.RequestDisambiguationAsync(response);

            //Create dialogue confirm that user selected
            msgback.DisplayMessage = msgback.SpokenMessage = "Are you sure you want select " + selectedRes.SelectedItem.Title + " ?";
            msgRepeat.DisplayMessage = msgRepeat.SpokenMessage = "Please select Yes or No";
            response = VoiceCommandResponse.CreateResponseForPrompt(msgback, msgRepeat);

            //return YES OR NO
            var result = await voiceServiceConnection.RequestConfirmationAsync(response);
            if (result.Confirmed)
            {
                if (selectedRes.SelectedItem.AppLaunchArgument == "more")
                {

                    await CortanaList(destinationContentTiles, cmdList, cmtList,commandsTook,commandsCountingNo);

                    msgback.DisplayMessage = msgback.SpokenMessage = $"Please use Cortana to select voice command {selectedRes.SelectedItem.Title}";
                    msgRepeat.DisplayMessage = msgRepeat.SpokenMessage = $"Please use Cortana to select voice command {selectedRes.SelectedItem.Title}";
                    response = VoiceCommandResponse.CreateResponseForPrompt(msgback, msgRepeat);
                    return response;
                }

                msgback.DisplayMessage = msgback.SpokenMessage = $"Please use Cortana to select voice command {selectedRes.SelectedItem.Title}";
                msgRepeat.DisplayMessage = msgRepeat.SpokenMessage = $"Please use Cortana to select voice command {selectedRes.SelectedItem.Title}";
                response = VoiceCommandResponse.CreateResponseForPrompt(msgback, msgRepeat);
            }
            else
            {
                await Task.Delay(3000);
                await CortanaHelpList();
            }

            return response;
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
