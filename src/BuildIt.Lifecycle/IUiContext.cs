using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace BuildIt.Lifecycle
{
    public interface IUIExecutionContext
    {
        Task RunOnUIThreadAsync(Func<Task> action);

        bool IsRunningOnUIThread { get; }
    }

    public interface IRequiresUIAccess
    {
        UIExecutionContext UIContext { get; }
    }
}