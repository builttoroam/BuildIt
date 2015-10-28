using System;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;

namespace BuildIt.Lifecycle
{
    public class UIContext
    {
        private IUIContext runContext;
        private IUIContext RunContext => runContext ?? (runContext = ServiceLocator.Current.GetInstance<IUIContext>());

        public async Task RunAsync(Action action)
        {
#pragma warning disable 1998 // Required to force to Task overloaded method
            await RunAsync(async () => action());
#pragma warning restore 1998
        }

        public async Task RunAsync(Func<Task> action)
        {
            var context = RunContext;
            await context.RunOnUIThreadAsync(action);
        }
    }
}