using Android.App;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.UI
{
    /// <summary>
    /// Implements UI abstraction.
    /// </summary>
    public class PlatformUIContext : IUIExecutionContext
    {
        /// <inheritdoc/>
        public bool IsRunningOnUIThread => Application.SynchronizationContext == SynchronizationContext.Current;

        /// <inheritdoc/>
        public async Task RunOnUIThreadAsync(Func<Task> action)
        {
            if (IsRunningOnUIThread)
            {
                await action();
            }
            else
            {
                Application.SynchronizationContext.Post(
                    async ignored =>
                {
                    await action();
                }, null);
            }
        }
    }
}