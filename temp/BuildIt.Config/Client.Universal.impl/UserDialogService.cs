using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using BuildIt.Config.Core.Standard.Services.Interfaces;
using Client.Universal.Impl.Extensions;

namespace Client.Universal.Impl
{
    public class UserDialogService : IUserDialogService
    {

        private CoreDispatcher dispatcher;

        private CoreDispatcher Dispatcher
        {
            get
            {
                if (dispatcher == null)
                {
                    dispatcher = CoreWindow.GetForCurrentThread()?.Dispatcher;
                }
                return dispatcher;
            }
        }

        public async Task AlertAsync(string message, string title = null, string okText = null, CancellationToken? cancelToken = null)
        {
            if (Dispatcher == null) return;

            await Dispatcher.RunTaskAsync(async () =>
            {
                var dialog = new MessageDialog(message) { Title = title ?? "" };
                await dialog.ShowAsync();
            });
        }
    }
}
