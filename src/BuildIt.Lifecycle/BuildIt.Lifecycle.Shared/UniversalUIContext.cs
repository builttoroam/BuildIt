using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace BuildIt.Lifecycle
{
    public class UniversalUIContext : IUIExecutionContext
    {
        private CoreDispatcher Dispatcher { get; set; }

        public UniversalUIContext(CoreDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        public async Task RunOnUIThreadAsync(Func<Task> action)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await action());
             
        }

        public bool IsRunningOnUIThread => Dispatcher.HasThreadAccess;
    }
}