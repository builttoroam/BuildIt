using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using BuildIt.Config.Core.Standard.Services.Interfaces;
using BuildIt.Config.Core.Standard.Utilities;

namespace Client.Android.Impl
{
    public class UserDialogService : IUserDialogService
    {
        private IUserDialogs acrUserDialogs;
        private IUserDialogs AcrUserDialogs
        {
            get
            {
                if (acrUserDialogs == null)
                {
                    UserDialogs.Init(Context);
                    acrUserDialogs = UserDialogs.Instance;
                }

                return acrUserDialogs;
            }
        }

        public Activity Context { get; set; }

        /// <summary>
        /// Ensure that Context is set before calling GetVersion()
        /// e.g. versionService.Context = ApplicationContext; (called from MainActivity)
        /// </summary>        
        public async Task AlertAsync(string message, string title = null, string okText = null, CancellationToken? cancelToken = null)
        {
            if (Context == null) return;

            var alertAsync = AcrUserDialogs?.AlertAsync(message, title, okText, cancelToken);
            if (alertAsync != null) await alertAsync;

            //var taskCompletionSource = new TaskCompletionSource<Task>();

            //using (var h = new Handler(Looper.MainLooper))
            //{
            //    h.Post(() =>
            //    {
            //        title = !string.IsNullOrWhiteSpace(title) ? title : Strings.DefaultAlertTitle;
            //        okText = !string.IsNullOrWhiteSpace(okText) ? okText : Strings.Ok;

            //        var alert = new AlertDialog.Builder(Context);
            //        alert.SetTitle(title);
            //        alert.SetMessage(message);
            //        alert.SetPositiveButton(okText, (senderAlert, args) =>
            //        {
            //            taskCompletionSource.SetResult(Task.FromResult(0));
            //        });

            //        AlertDialog dialog = alert.Create();
            //        dialog.Show();
            //    });
            //}

            //await taskCompletionSource.Task;
        }
    }
}