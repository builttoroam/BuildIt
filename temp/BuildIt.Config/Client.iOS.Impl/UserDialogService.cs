using System;
using System.Threading;
using System.Threading.Tasks;
using BuildIt.Config.Core.Standard.Services.Interfaces;
using Foundation;
using UIKit;

namespace Client.iOS.Impl
{
    public class UserDialogService : IUserDialogService
    {
        public Task AlertAsync(string message, string title = null, string okText = null, CancellationToken? cancelToken = null)
        {
            var tcs = new TaskCompletionSource<bool>();
            UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                var alert = new UIAlertView(title, message, null,
                    NSBundle.MainBundle.LocalizedString("OK", "OK"));
                alert.Clicked += (sender, buttonArgs) => tcs.SetResult(buttonArgs.ButtonIndex == alert.CancelButtonIndex);
                alert.Show();
            });

            return tcs.Task;
        }
    }
}
