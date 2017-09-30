using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.Lifecycle
{
    /// <summary>
    /// Reference to UI context for code execution
    /// </summary>
    public class FormsUIContext : IUIExecutionContext
    {
        /// <summary>
        /// Gets a value indicating whether gets a value indicating if running on the UI thread
        /// </summary>
        public bool IsRunningOnUIThread => false;

        /// <summary>
        /// Invokes the action on the UI thread
        /// </summary>
        /// <param name="action">The action to invoke</param>
        /// <returns>Task to await</returns>
        public async Task RunOnUIThreadAsync(Func<Task> action)
        {
            var waiter = new ManualResetEvent(false);

            Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
            {
                await action();
                waiter.Set();
            });
            await Task.Run(() => waiter.WaitOne());

            // Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            // {
            //    //  var waiter = new ManualResetEvent(false);
            //    action().Wait();
            //    //waiter.Set();
            // });
            ////await Task.Run(() => waiter.WaitOne());
        }
    }
}