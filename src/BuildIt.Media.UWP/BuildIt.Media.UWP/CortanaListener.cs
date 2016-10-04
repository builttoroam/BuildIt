using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Media.SpeechRecognition;
using Windows.Storage;

namespace BuildIt.Media
{

    public static class CortanaListener
    {
        private static class VoiceCommandSchema
        {
            public static XName CommandSet => VoiceCommandNameSpace.GetName("CommandSet");
            public static class CommandSetElement
            {
                
                public static XName AppName => VoiceCommandNameSpace.GetName("AppName");
            }
        }

        private const string LaunchContext = "LaunchContext";

        private const string BuildItVoiceCommandFileName = "Voices.xml";
        private static XNamespace VoiceCommandNameSpace { get; }= XNamespace.Get("http://schemas.microsoft.com/voicecommands/1.2");
        private static XNamespace XmlNameSpace { get; } = XNamespace.Get("http://www.w3.org/XML/1998/namespace");

        public static async Task RegisterMediaElementVoiceCommands(string customVoiceCommandFileName = null, bool registerMissingLocales = true)
        {
            try
            {
                // Define a temporary voice file into which the BuildIt voice command will be copied
                // If a custom voice command file is specified, it will be merged with the BuildIt voice command before 
                // being written into the temporary voice file
                var tempVoiceFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(BuildItVoiceCommandFileName, CreationCollisionOption.ReplaceExisting);


                var assembly = typeof(CortanaListener).GetTypeInfo().Assembly;
                using (var stream = assembly.GetManifestResourceStream(typeof(CortanaListener).Namespace + "." + BuildItVoiceCommandFileName))
                using (var outStream = await tempVoiceFile.OpenStreamForWriteAsync())
                {
                    var defaultXml = XDocument.Load(stream);
                    var customXml = defaultXml;
                    if (!string.IsNullOrWhiteSpace(customVoiceCommandFileName))
                    {
                        // A custom voice command file has been provided, so need to combine the commands

                        // Load the custom voice commands
                        customXml = XDocument.Load(customVoiceCommandFileName);


                        var buildItCommandSets = (from c in defaultXml.Descendants()
                                              where VoiceCommandSchema.CommandSet == c.Name
                                              select c).ToList();
                        var customCommandSets = (from c in customXml.Descendants()
                                                    where VoiceCommandSchema.CommandSet == c.Name
                                                    select c).ToList();

                        
                        //var commandList = (from c in defaultXml.Descendants()
                        //    where VoiceCommandNameSpace.GetName("Command") == c.Name
                        //    select c).ToList();

                        //var cutomCommandList = (from c in customXml.Descendants()
                        //    where VoiceCommandNameSpace.GetName("Command") == c.Name
                        //    select c).ToList();

                        //var commandWithServiceList = (from c in commandList.Descendants()
                        //    where VoiceCommandNameSpace.GetName("VoiceCommandService") == c.Name
                        //    select c).ToList();

                        //var customCommandWithService = (from c in cutomCommandList.Descendants()
                        //    where VoiceCommandNameSpace.GetName("VoiceCommandService") == c.Name
                        //    select c).FirstOrDefault();
                        
                        //if (!string.IsNullOrEmpty(appServiceName))
                        //{
                        //    foreach (var command in commandWithServiceList)
                        //    {
                        //        command.Attribute("Target").Value = appServiceName;
                        //    }
                        //}
                        //else
                        //{
                        //    appServiceName = customCommandWithService.Attribute("Target").Value;

                        //    foreach (var command in commandWithServiceList)
                        //    {
                        //        command.Attribute("Target").Value = appServiceName;
                        //    }
                        //}


                        //var appName = (from c in customXml.Descendants()
                        //    where VoiceCommandNameSpace.GetName("CommandSet") == c.Name
                        //    where c.Attribute(xmlns.GetName("lang")).Value == currentLocation
                        //    let appNameCommandSetDesc = c.Descendants()
                        //    from ac in appNameCommandSetDesc
                        //    where VoiceCommandNameSpace.GetName("AppName") == ac.Name
                        //    select ac.Value).FirstOrDefault();

                        //set commandSetList AppName to be appname if appname is not null 
                        //var appName = Package.Current.DisplayName;
                        //if (!string.IsNullOrEmpty(appName))
                        //{
                        //    appName = Package.Current.DisplayName;

                            
                        //    foreach (var command in buildItCommandSets)
                        //    {
                        //        var appNameNode = (from c in command.Descendants()
                        //                           where VoiceCommandNameSpace.GetName("AppName") == c.Name
                        //                           select c).FirstOrDefault();
                        //        appNameNode.Value = appName;
                        //    }
                        //}



                        var rootVoiceCommandsNode = customXml.FirstNode as XElement;
                        if (rootVoiceCommandsNode == null)
                        {
                            rootVoiceCommandsNode = new XElement(VoiceCommandNameSpace.GetName("VoiceCommands"));
                            customXml.Add(rootVoiceCommandsNode);
                        }

                        foreach (var element in buildItCommandSets)
                        {
                            var matchFound = false;
                            foreach (var customElement in customCommandSets)
                            {
                                //add command nodes if the commandSet is existing
                                if (element.Attribute(XmlNameSpace.GetName("lang")).Value !=
                                    customElement.Attribute(XmlNameSpace.GetName("lang")).Value) continue;

                                var commandNodes = (from c in element.Descendants()
                                                    where VoiceCommandNameSpace.GetName("Command") == c.Name
                                                    select c).ToList();
                                customElement.Add(commandNodes);
                                matchFound = true;
                                break;
                            }

                            if (!matchFound)
                            {
                                rootVoiceCommandsNode.Add(element);
                            }
                        }


                        var allVoiceCommands = (from command in customXml.Descendants(VoiceCommandNameSpace.GetName("Command"))
                                                select command.SafeAttributeValue("Name")
                            ).Distinct().ToList();
                        foreach (var commandName in allVoiceCommands)
                        {
                            await CopyActionIconsToTempFolder(commandName + ".png");
                        }
                        //add custom tile image
                        await CopyActionIconsToTempFolder("buildit_customTile.png");



                        // Get the name of the registered background app service for the app
                        // The BuildIt background taks has to be registered with this name
                        var inventoryService = new AppServiceConnection();
                        var appServiceName = inventoryService.AppServiceName;

                        var voiceCommandServiceNodes = (from c in customXml.Descendants()
                                                        where VoiceCommandNameSpace.GetName("VoiceCommandService") == c.Name
                                                        select c).ToList();
                        voiceCommandServiceNodes.DoForEach(node =>
                        {
                            var attribute = node.Attribute("Target");
                            if (attribute == null)
                            {
                                attribute = new XAttribute("Target", appServiceName);
                                node.Add(attribute);
                            }
                            else
                            {
                                attribute.Value = appServiceName;
                            }
                        });
                    }

                    // We need to make sure the app name
                    // element is updated with the name of the app (ie display name in the package)

                    var appName = Package.Current.DisplayName;
                    var appNameNodes = (from c in defaultXml.Descendants()
                                        where VoiceCommandSchema.CommandSetElement.AppName == c.Name
                                        select c).ToList();
                    appNameNodes.DoForEach(appNameNode => appNameNode.Value = appName);

                    //save customXml 
                    customXml.Save(outStream);

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
