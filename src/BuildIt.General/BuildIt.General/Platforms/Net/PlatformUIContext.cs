using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace BuildIt.UI
{
    /// <summary>
    /// Implements UI abstraction.
    /// </summary>
    public class PlatformUIContext : IUIExecutionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformUIContext"/> class.
        /// </summary>
        /// <param name="dispatcher">The core dispatcher for UI actions.</param>
        public PlatformUIContext(Dispatcher dispatcher = null)
        {
            Dispatcher = dispatcher ?? Application.Current.Dispatcher;
        }

        /// <inheritdoc/>
        public bool IsRunningOnUIThread => Dispatcher.CurrentDispatcher.CheckAccess();

        private Dispatcher Dispatcher { get; }

        /// <inheritdoc/>
        public async Task RunOnUIThreadAsync(Func<Task> action)
        {
            if (IsRunningOnUIThread)
            {
                await action();
            }
            else
            {
                await Dispatcher.InvokeAsync(async () =>
                {
                    await action();
                });
            }
        }
    }
}