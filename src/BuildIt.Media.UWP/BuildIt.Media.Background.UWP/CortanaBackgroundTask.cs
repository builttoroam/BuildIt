using Windows.ApplicationModel.Background;

namespace BuildIt.Media.Background
{
    public sealed class CortanaBackgroundTask :  IBackgroundTask
    {
        private BaseCortanaBackgroundTask BackgroundTaskImplement { get; }= new BaseCortanaBackgroundTask();


        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            await BackgroundTaskImplement.Run(taskInstance);
        }
    }
}