using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.Lifecycle
{
    public class FormsUIContext : IUIExecutionContext
    {

        public async Task RunOnUIThreadAsync(Func<Task> action)
        {
            var waiter = new ManualResetEvent(false);

            Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
            {
                await action();
                waiter.Set();
            });
            await Task.Run(() => waiter.WaitOne());



            //Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            //{
            //    //  var waiter = new ManualResetEvent(false);
            //    action().Wait();
            //    //waiter.Set();
            //});
            ////await Task.Run(() => waiter.WaitOne());
        }

        public bool IsRunningOnUIThread => false;
    }
}