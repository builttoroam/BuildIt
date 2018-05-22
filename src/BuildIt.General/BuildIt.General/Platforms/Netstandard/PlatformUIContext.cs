using System;
using System.Threading.Tasks;

namespace BuildIt.UI
{
    /// <summary>
    /// Implements UI abstraction
    /// </summary>
    public class PlatformUIContext : IUIExecutionContext
    {
        /// <inheritdoc/>
        public bool IsRunningOnUIThread => throw new Exception("Make sure you reference the platform specific library");

        /// <inheritdoc/>
        public Task RunOnUIThreadAsync(Func<Task> action) => throw new Exception("Make sure you reference the platform specific library");
    }
}