using System;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace BuildIt.Lifecycle
{
    /// <summary>
    /// Holds a reference to the UI context
    /// </summary>
    public class UniversalUIContext : IUIExecutionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UniversalUIContext"/> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher to invoke actions on</param>
        public UniversalUIContext(CoreDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        /// <summary>
        /// Gets a value indicating whether gets whether code is running on UI thread
        /// </summary>
        public bool IsRunningOnUIThread => Dispatcher.HasThreadAccess;

        private CoreDispatcher Dispatcher { get; }

        /// <summary>
        /// Invokes action on the UI thread
        /// </summary>
        /// <param name="action">The action to perform</param>
        /// <returns>Task to await</returns>
        public async Task RunOnUIThreadAsync(Func<Task> action)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await action());
        }
    }
}