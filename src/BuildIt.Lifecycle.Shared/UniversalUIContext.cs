using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace BuildIt.Lifecycle
{
    public class UniversalUIContext : IUIContext
    {
        public async Task RunOnUIThreadAsync(Func<Task> action)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await action());
             
        }

    }
}