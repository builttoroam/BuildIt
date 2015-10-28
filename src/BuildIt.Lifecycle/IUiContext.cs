using System;
using System.Threading.Tasks;

namespace BuildIt.Lifecycle
{
    public interface IUIContext
    {
        Task RunOnUIThreadAsync(Func<Task> action);
    }
}