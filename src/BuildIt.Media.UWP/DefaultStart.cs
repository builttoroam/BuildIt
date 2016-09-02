using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;
using BuildIt.General;

namespace BuildIt.Media
{

    public static class CortanaListener
    {
        public static async Task RegisterMediaElementVoiceCommands(string voiceCommandFileName = null, bool registerMissingLocales = true)
        {
            try
            {


                var assembly = typeof(CortanaListener).GetTypeInfo().Assembly;
                var tempVoiceFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("_voices.xml", CreationCollisionOption.ReplaceExisting);
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
                                customElement.Add(commandNodes);
                                matchFound = true;
                                break;
                            }

                            if (!matchFound) 
                            {
                                rootVoiceCommandsNode.Add(element);
                                break;
                            }
                        }


                        //foreach (var element in commandSetList)
                        //{
                        //    for (int i = 0; i <= customCommandSetList.Count - 1; i++)
                        //    {
                        //        //add command nodes if the commandSet is existing
                        //        if (element.Attribute(xmlns.GetName("lang")).Value == customCommandSetList[i].Attribute(xmlns.GetName("lang")).Value)
                        //        {
                        //            var commandSetDesc = from c in customXml.Descendants()
                        //                                 where ns.GetName("CommandSet") == c.Name
                        //                                 select c;
                        //            var commandNodes = (from c in element.Descendants()
                        //                                where ns.GetName("Command") == c.Name
                        //                                select c).ToList();
                        //            commandSetDesc.ElementAt(i).Add(commandNodes);
                        //            break;
                        //        }

                        //        //if commendSet not existing then add commandSet
                        //        if (i != customCommandSetList.Count - 1) continue;
                        //        {
                        //            var commandSetDesc = customXml.Descendants().Where(x => ns.GetName("CommandSet") == x.Name);
                        //            commandSetDesc.Last().AddAfterSelf(element);
                        //            break;
                        //        }
                        //    }
                        //}
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

        public static async Task<bool> HandleMediaElementCortanaCommands(this IActivatedEventArgs args)
        {
            try
            {
                if (args.Kind != ActivationKind.VoiceCommand) return false;

                var commandArgs = args as VoiceCommandActivatedEventArgs;
                var destination = commandArgs?.Result?.RulePath.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(destination)) return false;
                return await PlayerControls.Action(destination);
            }
            catch (Exception ex)
            {
                ex.LogException();
                return false;
            }
        }



    }
}
