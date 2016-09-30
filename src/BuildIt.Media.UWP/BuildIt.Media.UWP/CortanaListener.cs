using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Media.SpeechRecognition;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Buffer = System.Buffer;

namespace BuildIt.Media
{

    public static class CortanaListener
    {
        private const string LaunchContext = "LaunchContext";

        public static async Task RegisterMediaElementVoiceCommands(string voiceCommandFileName = null, bool registerMissingLocales = true)
        {
            try
            {
                //await CopyActionIconsToTempFolder("buildit_back.jpg");
                //await CopyActionIconsToTempFolder("buildit_forward.jpg");

                //var builditBack = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///BuildIt.Media/Image/buildit_back.jpg"));
                //await builditBack.CopyAsync(ApplicationData.Current.TemporaryFolder, "buildit_back.jpg");
                //var builditForward = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///BuildIt.Media/Image/buildit_forward.jpg"));
                //await builditForward.CopyAsync(ApplicationData.Current.TemporaryFolder, "buildit_forward.jpg");
                //var builditMute = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///BuildIt.Media/Image/buildit_mute.jpg"));
                //await builditMute.CopyAsync(ApplicationData.Current.TemporaryFolder, "buildit_mute.jpg");
                //var builditPause = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///BuildIt.Media/Image/buildit_pause.jpg"));
                //await builditPause.CopyAsync(ApplicationData.Current.TemporaryFolder, "buildit_pause.jpg");
                //var builditPlay = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///BuildIt.Media/Image/buildit_play.jpg"));
                //await builditPlay.CopyAsync(ApplicationData.Current.TemporaryFolder, "buildit_play.jpg");
                //var builditUnmute = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///BuildIt.Media/Image/buildit_unmute.jpg"));
                //await builditUnmute.CopyAsync(ApplicationData.Current.TemporaryFolder, "buildit_unmute.jpg");
                //var builditVolumeDown = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///BuildIt.Media/Image/buildit_volumeDown.jpg"));
                //await builditVolumeDown.CopyAsync(ApplicationData.Current.TemporaryFolder, "buildit_volumeDown.jpg");
                //var builditVolumeUp = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///BuildIt.Media/Image/buildit_volumeUp.jpg"));
                //await builditVolumeUp.CopyAsync(ApplicationData.Current.TemporaryFolder, "buildit_volumeUp.jpg");


                var assembly = typeof(CortanaListener).GetTypeInfo().Assembly;
                var tempVoiceFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("_voices.xml", CreationCollisionOption.ReplaceExisting);

                //var temback = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("buildit_back.jpg", CreationCollisionOption.FailIfExists);
                //var backStream = assembly.GetManifestResourceStream(typeof(CortanaListener).Namespace + "buildit_volumeDown.jpg");


                //using (IRandomAccessStream fileStream = await temback.OpenAsync(FileAccessMode.ReadWrite))
                //{
                //    using (IOutputStream outputStream = fileStream.GetOutputStreamAt(0))
                //    {
                //        using (DataWriter dataWriter = new DataWriter(outputStream))
                //        {
                //            dataWriter.WriteBytes();
                //            await dataWriter.StoreAsync();
                //            dataWriter.DetachStream();
                //        }
                //        await outputStream.FlushAsync();
                //    }
                //}









                using (
                    var stream = assembly.GetManifestResourceStream(typeof(CortanaListener).Namespace + ".Voices.xml"))
                using (var outStream = await tempVoiceFile.OpenStreamForWriteAsync())
                {
                    if (string.IsNullOrWhiteSpace(voiceCommandFileName))
                    {
                        await stream.CopyToAsync(outStream);
                    }
                    else
                    {
                        // TODO: Combine commands in the resources voice file with the file supplied as a parameter
                        var defaultXml = XDocument.Load(stream);
                        var customXml = XDocument.Load(voiceCommandFileName);
                        /*var elements = from c in customXml.Descendants()
                                       where  c.Name.LocalName == "Command"
                                       select c;*/
                        var ns = XNamespace.Get("http://schemas.microsoft.com/voicecommands/1.2");
                        var xmlns = XNamespace.Get("http://www.w3.org/XML/1998/namespace");
                        var commandSetList = (from c in defaultXml.Descendants()
                                              where ns.GetName("CommandSet") == c.Name
                                              select c).ToList();
                        var customCommandSetList = (from c in customXml.Descendants()
                                                    where ns.GetName("CommandSet") == c.Name
                                                    select c).ToList();

                        var rootVoiceCommandsNode = customXml.FirstNode as XElement;
                        if (rootVoiceCommandsNode == null)
                        {
                            rootVoiceCommandsNode = new XElement(ns.GetName("VoiceCommands"));
                            customXml.Add(rootVoiceCommandsNode);
                        }

                        foreach (var element in commandSetList)
                        {
                            var matchFound = false;
                            foreach (var customElement in customCommandSetList)
                            {
                                //add command nodes if the commandSet is existing
                                if (element.Attribute(xmlns.GetName("lang")).Value !=
                                    customElement.Attribute(xmlns.GetName("lang")).Value) continue;

                                var commandNodes = (from c in element.Descendants()
                                                    where ns.GetName("Command") == c.Name
                                                    select c).ToList();
                                var lastCommand = (from c in customElement.Descendants()
                                                   where ns.GetName("Command") == c.Name
                                                   select c).LastOrDefault();
                                if (lastCommand != null)
                                {
                                    lastCommand.AddAfterSelf(commandNodes);
                                }
                                else
                                {
                                    customElement.Add(commandNodes);
                                }
                                matchFound = true;
                                break;
                            }

                            if (!matchFound)
                            {
                                rootVoiceCommandsNode.Add(element);
                            }
                        }


                        var allVoiceCommands = (from command in customXml.Descendants(ns.GetName("Command"))
                                                select command.SafeAttributeValue("Name")
                            ).Distinct().ToList();
                        foreach (var commandName in allVoiceCommands)
                        {
                            await CopyActionIconsToTempFolder(commandName + ".jpg");
                        }


                        //save customXml 
                        customXml.Save(outStream);
                    }
                }
                await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(tempVoiceFile);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        private static async Task CopyActionIconsToTempFolder(string iconFileName)
        {
            try
            {
                // Start by opening the image that is packaged as an embedded resource
                var assembly = typeof(CortanaListener).GetTypeInfo().Assembly;
                using (
                    var stream =
                        assembly.GetManifestResourceStream(typeof(CortanaListener).Namespace + ".Images." + iconFileName)
                    )
                {
                    if (stream == null) return;
                    // Create the image in the local folder so that it can be used as an icon in Cortana interface
                    var iconFolder =
                        await
                            ApplicationData.Current.LocalFolder.CreateFolderAsync("builditmedia",
                                CreationCollisionOption.OpenIfExists);
                    var localIconFile =
                        await iconFolder.CreateFileAsync(iconFileName, CreationCollisionOption.ReplaceExisting);
                    using (var outStream = await localIconFile.OpenStreamForWriteAsync())
                    {
                        await stream.CopyToAsync(outStream);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }


        public static
            async Task<bool> HandleMediaElementCortanaCommands(this IActivatedEventArgs args)
        {
            try
            {
                switch (args.Kind)
                {
                    case ActivationKind.VoiceCommand:
                        var commandArgs = args as VoiceCommandActivatedEventArgs;

                        var voiceCommandName = commandArgs?.Result?.RulePath.FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(voiceCommandName)) return false;
                        return await PlayerControls.Action(voiceCommandName);
                    case ActivationKind.Protocol:
                        var voiceCommaneNameUri = string.Empty;
                        var commandArgsUris = args as ProtocolActivatedEventArgs;
                        var uri = commandArgsUris?.Uri.AbsoluteUri.Split(new[] { '?' }, 2);
                        if (uri != null)
                        {
                            var keyValuePairs = uri[1].Split('&')
                                .Select(x => x.Split('='))
                                .Where(x => x.Length == 2)
                                .ToDictionary(x => x.First(), x => x.Last());
                            if (keyValuePairs.ContainsKey(LaunchContext))
                            {
                                voiceCommaneNameUri = keyValuePairs[LaunchContext];
                            }
                        }
                        //var words = uri?[1].Split(new char[] { '=' }, 2);
                        //var vCommandNameUri = words?[1];
                        //var voiceCommaneNameUri = commandArgsUris?.Uri.AbsoluteUri.Split(new char[] { '?' }, 2)?[1].Split(new char[] { '=' }, 2)?[1];
                        if (string.IsNullOrWhiteSpace(voiceCommaneNameUri)) return false;
                        return await PlayerControls.Action(voiceCommaneNameUri);
                    default:
                        return false;
                }
                //if (args.Kind != ActivationKind.VoiceCommand) return false;
                //var test = args as ProtocolActivatedEventArgs;
                //var commandArgs = args as VoiceCommandActivatedEventArgs;

                //var voiceCommandName = commandArgs?.Result?.RulePath.FirstOrDefault();
            }
            catch (Exception ex)
            {
                ex.LogException();
                return false;
            }
        }

        /// <summary>
        /// Returns the semantic interpretation of a speech result. 
        /// Returns null if there is no interpretation for that key.
        /// </summary>
        /// <param name="interpretationKey">The interpretation key.</param>
        /// <param name="speechRecognitionResult">The speech recognition result to get the semantic interpretation from.</param>
        /// <returns></returns>
        private static string SemanticInterpretation(string interpretationKey, SpeechRecognitionResult speechRecognitionResult)
        {
            return speechRecognitionResult.SemanticInterpretation.Properties[interpretationKey].FirstOrDefault();
        }
    }
}
