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
            msgRepeat.DisplayMessage = msgRepeat.SpokenMessage = "Please select one";

            var userMessage = new VoiceCommandUserMessage();
            userMessage.DisplayMessage = "Here is the help list for you";
            userMessage.SpokenMessage = "Here is the help list for you";
            var commandsTook = 0;
            var commandsCountingNo = 0;


            var storageFile = await Package.Current.InstalledLocation.GetFileAsync("assets\\buildit_forward.jpg");

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
            var command = (from c in commandSet.Descendants()
                           where ns.GetName("Command") == c.Name

                           select c);
            var commandList = command.ToList();

            var test = (from c in commandList.Descendants()
                        where c.NodeType == XmlNodeType.Comment
                        select c).ToList();


            var cmtList = command.DescendantNodes().OfType<XComment>().ToList();

            var response = await CortanaList(destinationContentTiles, commandList, cmtList,commandsTook,commandsCountingNo);

            await voiceServiceConnection.ReportSuccessAsync(response);
        }

        private async Task<VoiceCommandResponse> CortanaList(List<VoiceCommandContentTile> destinationContentTiles, List<XElement> commandList, List<XComment> commentList ,int cmdsTook,int cmdCountingNo)
        {
            var destContentTiles = destinationContentTiles;
            var cmdList = commandList;
            var cmtList = commentList;
            var commandsTook = cmdsTook;
            var commandsCountingNo = cmdCountingNo;
            var ns = XNamespace.Get("http://schemas.microsoft.com/voicecommands/1.2");
            //back for cortana to show the content
            var msgback = new VoiceCommandUserMessage();
            msgback.DisplayMessage = msgback.SpokenMessage = "Here is the help list for you";
            //Cortana 
            var msgRepeat = new VoiceCommandUserMessage();
            msgRepeat.DisplayMessage = msgRepeat.SpokenMessage = "Please select one";
            var userMessage = new VoiceCommandUserMessage();
            userMessage.DisplayMessage = "Here is the help list for you";
            userMessage.SpokenMessage = "Here is the help list for you";

            var moreCommands = "Select next page commands";

            var iconsFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("builditmedia");
            destContentTiles.Clear();
            if (cmdList.Count - commandsTook <= 5)
            {
                for (int i = commandsCountingNo; i <= cmdList.Count-1; i++)
                {
                    var command = cmdList[commandsTook];
                    //var cmtName = cmtList[commandsTook].Parent.Attribute("Name").Value;
                    var attributeName = command.Attribute("Name").Value;

                    var descriptionComment = (from comment in command.DescendantNodes().OfType<XComment>()
                                              where comment.Value.StartsWith("Description:")
                                              select comment.Value)
                                              .FirstOrDefault() // Get the first comment that starts with "Description:"
                                              ?.Replace("Description:", ""); // If one exists, trim "Description:" by replacing it with ""

                    var txtline1 = "test";
                    if (attributeName.Contains("buildit") != true)
                    {
                        attributeName = "artwork";
                    }

                    var iconFile = await iconsFolder.GetFileAsync("buildit_pause.jpg");

                    destContentTiles.Add(new VoiceCommandContentTile
                    {
                        AppLaunchArgument = cmdList[commandsTook].Attribute("Name").Value,
                        ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                        Title = cmdList[commandsTook].Element(ns.GetName("Example")).Value,
                        TextLine1 = txtline1,
                        Image = iconFile
                    });
                    commandsTook++;
                }
            }
            else
            {
                destContentTiles.Clear();
                for (int i = 0; i < 4; i++)
                {
                    var command = cmdList[commandsTook];
                    //var cmtName = cmtList[commandsTook].Parent.Attribute("Name").Value;
                    var attributeName = cmdList[commandsTook].Attribute("Name").Value;
                    //var txtline1 = cmtName == attributeName ? cmtList[commandsTook].Value : "";
                    var descriptionComment = (from comment in command.DescendantNodes().OfType<XComment>()
                                              where comment.Value.StartsWith("Description:")
                                              select comment.Value)
                      .FirstOrDefault() // Get the first comment that starts with "Description:"
                      ?.Replace("Description:", ""); // If one exists, trim "Description:" by replacing it with ""
                    if (descriptionComment == null)
                    {
                        descriptionComment = "";
                    }
                    if (attributeName.Contains("buildit") != true)
                    {
                        attributeName = "artwork";
                    }

                    var iconFile = await iconsFolder.GetFileAsync("buildit_pause.jpg");


                    destContentTiles.Add(new VoiceCommandContentTile
                    {
                        AppLaunchArgument = cmdList[commandsTook].Attribute("Name").Value,
                        ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                        Title = cmdList[commandsTook].Element(ns.GetName("Example")).Value,
                        TextLine1 = descriptionComment,
                        Image = iconFile
                    });
                    commandsTook++;
                }
                var nextPage = new VoiceCommandContentTile
                {
                    ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                    Title = "More voice commands",
                    AppLaunchArgument = "more",
                    TextLine1 = moreCommands,
                    Image = await Package.Current.InstalledLocation.GetFileAsync($"images\\artwork.jpg")
                };
                destContentTiles.Add(nextPage);

                commandsCountingNo += 4;
            }


            // Cortana will handle re-prompting if the user does not provide a valid response.
            var response = VoiceCommandResponse.CreateResponseForPrompt(msgback, msgRepeat, destContentTiles);
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

                    await CortanaList(destContentTiles, cmdList, cmtList,commandsTook,commandsCountingNo);
                    //await ShowMoreVoiceCommand(cmdList, commandsCountingNo, destContentTiles,commandsTook, cmtList);
                 
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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<VoiceCommandResponse> ShowMoreVoiceCommand(List<XElement> commandList, int commandsCountingNo,
            List<VoiceCommandContentTile> destinationContentTiles, int commandsTook, List<XComment> cmtList)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var number = 1;
            var moreCommands = "Select next page commands";
            var cmdList = commandList;
            var cmdsNo = commandsCountingNo;
            var destContentTiles = destinationContentTiles;
            var cmdsTook = commandsTook;
            var ns = XNamespace.Get("http://schemas.microsoft.com/voicecommands/1.2");
            var msgback = new VoiceCommandUserMessage();
            msgback.DisplayMessage = msgback.SpokenMessage = "Here is the help list for you";
            //Cortana 
            var msgRepeat = new VoiceCommandUserMessage();
            msgRepeat.DisplayMessage = msgRepeat.SpokenMessage = "Please select one";



            if (cmdList.Count <= 4)
            {
                for (int i = cmdsNo; i < cmdList.Count - 1; i++)
                {
                    var cmtName = cmtList[commandsTook].Parent.Attribute("Name").Value;
                    var attributeName = commandList[commandsTook].Attribute("Name").Value;
                    var txtline1 = cmtName == attributeName ? cmtList[commandsTook].Value : "";
                    if (attributeName.Contains("buildit") != true)
                    {
                        attributeName = "artwork";
                    }
                    destContentTiles.Add(new VoiceCommandContentTile
                    {

                        AppLaunchArgument = cmdList[i].Attribute("Name").Value,
                        ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                        Title = cmdList[i].Element(ns.GetName("Example")).Value,
                        TextLine1 = txtline1,
                        Image = await Package.Current.InstalledLocation.GetFileAsync($"images\\{attributeName}.jpg")
                    });
                }
            }
            else
            {
                destContentTiles.Clear();
                if (cmdList.Count - cmdsNo > 4)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var cmtName = cmtList[commandsTook].Parent.Attribute("Name").Value;
                        var attributeName = commandList[commandsTook].Attribute("Name").Value;
                        var txtline1 = cmtName == attributeName ? cmtList[commandsTook].Value : "";
                        if (attributeName.Contains("buildit") != true)
                        {
                            attributeName = "artwork";
                        }
                        destContentTiles.Add(new VoiceCommandContentTile
                        {
                            AppLaunchArgument = cmdList[cmdsTook].Attribute("Name").Value,
                            ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                            Title = cmdList[cmdsTook].Element(ns.GetName("Example")).Value,
                            TextLine1 = txtline1,
                            Image = await Package.Current.InstalledLocation.GetFileAsync($"images\\{attributeName}.jpg")
                        });
                        cmdsTook++;
                    }
                    var nextPage = new VoiceCommandContentTile
                    {
                        ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                        Title = "More voice commands",
                        AppLaunchArgument = "more",
                        TextLine1 = moreCommands,
                        Image = await Package.Current.InstalledLocation.GetFileAsync($"images\\artwork.jpg")

                    };
                    destContentTiles.Add(nextPage);

                    cmdsNo += 4;
                }
                else
                {
                    destContentTiles.Clear();
                    for (int i = cmdsNo; i < cmdList.Count - 1; i++)
                    {

                        var cmtName = cmtList[commandsTook].Parent.Attribute("Name").Value;
                        var attributeName = commandList[commandsTook].Attribute("Name").Value;
                        var txtline1 = cmtName == attributeName ? cmtList[commandsTook].Value : "";
                        if (attributeName.Contains("buildit") != true)
                        {
                            attributeName = "artwork";
                        }

                        destContentTiles.Add(new VoiceCommandContentTile
                        {
                            AppLaunchArgument = cmdList[i].Attribute("Name").Value,
                            ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                            Title = cmdList[i].Element(ns.GetName("Example")).Value,
                            TextLine1 = txtline1,
                            Image = await Package.Current.InstalledLocation.GetFileAsync($"images\\{attributeName}.jpg")
                        });
                    }
                    if (cmdList.Count - cmdsNo >= 5)
                    {
                        var nextPage = new VoiceCommandContentTile
                        {
                            ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText,
                            Title = "More voice commands",
                            AppLaunchArgument = "more",
                            TextLine1 = moreCommands,
                            Image = await Package.Current.InstalledLocation.GetFileAsync($"assets\\artwork.jpg")
                        };

                        destContentTiles.Add(nextPage);
                    }
                }
            }
            var response = VoiceCommandResponse.CreateResponseForPrompt(msgback, msgRepeat, destContentTiles);
            // If cortana is dismissed in this operation, null will be returned.

            var selectedRes = await voiceServiceConnection.RequestDisambiguationAsync(response);

            //Create dialogue confirm that user selected
            msgback.DisplayMessage = msgback.SpokenMessage = "Are you sure you want select " + selectedRes.SelectedItem.Title + " ?";
            msgRepeat.DisplayMessage = msgRepeat.SpokenMessage = "Please select Yes or No";
            response = VoiceCommandResponse.CreateResponseForPrompt(msgback, msgRepeat);
            var result = await voiceServiceConnection.RequestConfirmationAsync(response);
            if (result.Confirmed)
            {
                if (selectedRes.SelectedItem.AppLaunchArgument == "more")
                {
                    destContentTiles.Clear();
                    number++;
                    await ShowMoreVoiceCommand(cmdList, cmdsNo, destContentTiles, cmdsTook, cmtList);
                }
                else
                {
                    msgback.DisplayMessage = msgback.SpokenMessage = $"Please use Cortana to select voice command {selectedRes.SelectedItem.Title}";
                    msgRepeat.DisplayMessage = msgRepeat.SpokenMessage = $"Please use Cortana to select voice command {selectedRes.SelectedItem.Title}";
                    response = VoiceCommandResponse.CreateResponseForPrompt(msgback, msgRepeat);
                    return response;
                }
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
