using System;
using System.Threading.Tasks;

namespace BuildIt.Lifecycle
{
    public class UIExecutionContext
    {
        public IUIExecutionContext RunContext { get; set; }

        public async Task RunAsync(Action action)
        {
#pragma warning disable 1998 // Required to force to Task overloaded method
            await RunAsync(async () => action());
#pragma warning restore 1998
        }

        public async Task RunAsync(Func<Task> action)
        {
            var context = RunContext;
            if (context.IsRunningOnUIThread)
            {
                await action();
            }
            else
            {
                await context.RunOnUIThreadAsync(action);
            }
        }
    }
}