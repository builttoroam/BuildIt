using Windows.ApplicationModel.Background;

namespace BuildIt.Media.Background
{
    public sealed class CortanaBackgroundTask :  IBackgroundTask
    {
        private BaseCortanaBackgroundTask BackgroundTaskImplement { get; }= new BaseCortanaBackgroundTask();


        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskImplement.Run(taskInstance);
        }
    }
}