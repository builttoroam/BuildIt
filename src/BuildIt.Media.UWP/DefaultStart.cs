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

namespace BuildIt.Media
{
   
    public static class CortanaListener
    {
        public static async Task RegisterMediaElementVoiceCommands(string voiceCommandFileName = null, bool registerMissingLocales=true)
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
                        var commandList = (from c in defaultXml.Descendants()
                                       where ns.GetName("Command")==c.Name 
                                       select c).ToList();
                        var descendents = customXml.Descendants().Where(x => ns.GetName("Command") == x.Name);
                        descendents.FirstOrDefault().AddAfterSelf(commandList);
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
