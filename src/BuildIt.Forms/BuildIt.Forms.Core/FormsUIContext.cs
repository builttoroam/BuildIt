using System;
using System.Threading.Tasks;

namespace BuildIt.Forms
{
    /// <summary>
    /// Forms execution context
    /// </summary>
    public class FormsUIContext : IUIExecutionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormsUIContext"/> class.
        /// </summary>
        /// <param name="platformContext">The actual platform context</param>
        public FormsUIContext(IUIExecutionContext platformContext)
        {
            PlatformContext = platformContext;
        }

        /// <inheritdoc/>
        public virtual bool IsRunningOnUIThread => PlatformContext.IsRunningOnUIThread;

        private IUIExecutionContext PlatformContext { get; }

        /// <inheritdoc/>
        public virtual Task RunOnUIThreadAsync(Func<Task> action) => PlatformContext.RunOnUIThreadAsync(action);
    }
}