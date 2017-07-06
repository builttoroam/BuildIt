using System;
using System.Threading.Tasks;

namespace BuildIt
{
    public interface IUIExecutionContext
    {
        /// <summary>
        /// NOTE: Don't call directly, use helper method RunAsync which uses IsRunningOnUIThread
        /// to determine whether switch to UI thread is necessary
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        Task RunOnUIThreadAsync(Func<Task> action);

        bool IsRunningOnUIThread { get; }
    }
}