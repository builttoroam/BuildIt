using System;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BuildIt.Config.Core.Services.Interfaces;

namespace BuildIt.Config.Impl.Common
{
    public class UserDialogService : IUserDialogService
    {
        private readonly IUserDialogs acrUserDialogs;

        public UserDialogService(IUserDialogs acrUserDialogs)
        {
            this.acrUserDialogs = acrUserDialogs;
        }

        public async Task AlertAsync(string message, string title = null, string okText = null, CancellationToken? cancelToken = null)
        {
            var alertAsync = acrUserDialogs?.AlertAsync(message, title, okText, cancelToken);
            if (alertAsync != null) await alertAsync;
        }
    }
}
