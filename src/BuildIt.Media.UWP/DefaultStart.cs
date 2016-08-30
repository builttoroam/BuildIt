using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CortanaControl
{
    public class DefaultStart : Application
    {
        private string voiceCommandString;
        private IActivatedEventArgs userVoiceCommands;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected override async void OnActivated(IActivatedEventArgs args)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            base.OnActivated(args);

            // Handle activation based on voice commands
            // var handled = await BuildIt.Player.Cortana.HandleActivation(args);
            // if (handled) return;
            if (userVoiceCommands != null)
            {
                if (UserVoiceCommands.Kind != ActivationKind.VoiceCommand) return;

                var commandArgs = args as VoiceCommandActivatedEventArgs;
                var destination = commandArgs?.Result?.RulePath[0];
                PlayerControls.Action(destination);
            }
            else
            {
                if (args.Kind != ActivationKind.VoiceCommand) return;

                var commandArgs = args as VoiceCommandActivatedEventArgs;
                var destination = commandArgs?.Result?.RulePath[0];
                PlayerControls.Action(destination);
            }
        }

        public IActivatedEventArgs UserVoiceCommands
        {
            get { return userVoiceCommands; }
            set { userVoiceCommands = value; }
        }

        public string VoiceCommandString
        {
            get { return voiceCommandString; }
            set { voiceCommandString = value; }
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Registering for voice commands
            // BuildIt.Player.Cortana.RegisterVoiceCommands(@"VoiceDefinitions.xml");
            try
            {
                StorageFile voices;
                if (string.IsNullOrEmpty(VoiceCommandString))
                {
                    voices = await Package.Current.InstalledLocation.GetFileAsync(@"DefaultVoices.xml");
                }
                else
                {
                    voices = await Package.Current.InstalledLocation.GetFileAsync(VoiceCommandString);
                }

                await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(voices);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Installing Voice Commands Failed: " + ex.ToString());
            }
        }

    }
}
