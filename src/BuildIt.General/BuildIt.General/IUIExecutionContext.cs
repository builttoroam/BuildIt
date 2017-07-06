using System;
using System.Threading.Tasks;

namespace BuildIt
{
    /// <summary>
    /// Allows entity to execute action on UI thread
    /// </summary>
    public interface IUIExecutionContext
    {
        /// <summary>
        /// Gets a value indicating whether whether the current thread is the UI thread
        /// </summary>
        bool IsRunningOnUIThread { get; }

        /// <summary>
        /// NOTE: Don't call directly, use helper method RunAsync which uses IsRunningOnUIThread
        /// to determine whether switch to UI thread is necessary
        /// </summary>
        /// <param name="action">The action to execute on UI thread</param>
        /// <returns>Task that can be awaited</returns>
        Task RunOnUIThreadAsync(Func<Task> action);
    }
}