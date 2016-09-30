using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuildIt.Config.Core.Services.Interfaces;

namespace BuildIt.Config.Impl.Common
{
    public class UserDialogService : IUserDialogService
    {
        public Task AlertAsync(string message, string title = null, string okText = null, CancellationToken? cancelToken = null)
        {
            throw new NotImplementedException();
        }
    }
}
