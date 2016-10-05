using System;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;

namespace BuildIt.Media.Background
{
    public sealed class CortanaBackgroundTask :  IBackgroundTask
    {
        private BaseCortanaBackgroundTask BackgroundTaskImplement { get; }= new BaseCortanaBackgroundTask();


        public async void Run(IBackgroundTaskInstance taskInstance)
        {

            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            if (triggerDetails == null)
            {
                
                return;
            }
            try
            {
                var voiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);
                await BackgroundTaskImplement.Run(taskInstance, voiceServiceConnection);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }
    }
}