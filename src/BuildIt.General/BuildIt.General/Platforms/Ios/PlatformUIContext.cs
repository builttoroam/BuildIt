using System;
using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace BuildIt.UI
{
    /// <summary>
    /// Implements UI abstraction.
    /// </summary>
    public class PlatformUIContext : IUIExecutionContext
    {
        /// <inheritdoc/>
        public bool IsRunningOnUIThread => Context == SynchronizationContext.Current;

        private SynchronizationContext Context { get; } = SynchronizationContext.Current;

        /// <inheritdoc/>
        public async Task RunOnUIThreadAsync(Func<Task> action)
        {
            if (IsRunningOnUIThread)
            {
                await action();
            }
            else
            {
                UIApplication.SharedApplication.BeginInvokeOnMainThread(async () =>
                {
                    await action();
                });
            }
        }
    }
}