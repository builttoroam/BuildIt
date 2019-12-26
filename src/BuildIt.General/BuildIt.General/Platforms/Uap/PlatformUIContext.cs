using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;

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
        public PlatformUIContext(CoreDispatcher dispatcher = null)
        {
#if UNO && !UAP
            Dispatcher = CoreDispatcher.Main;
#else
            Dispatcher = dispatcher ??
                ((Window.Current == null) ? CoreApplication.MainView.CoreWindow.Dispatcher : CoreApplication.GetCurrentView().CoreWindow.Dispatcher);
#endif
        }

        /// <inheritdoc/>
        public bool IsRunningOnUIThread => Dispatcher.HasThreadAccess;

        private CoreDispatcher Dispatcher { get; }

        /// <inheritdoc/>
        public async Task RunOnUIThreadAsync(Func<Task> action)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await action());
        }
    }
}