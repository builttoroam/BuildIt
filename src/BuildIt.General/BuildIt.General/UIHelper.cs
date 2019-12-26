using System;
using System.Threading.Tasks;

namespace BuildIt
{
    /// <summary>
    /// UI helper methods.
    /// </summary>
    public static class UIHelper
    {
        /// <summary>
        /// Register entity to access the UI context.
        /// </summary>
        /// <param name="requiresAccess">Entity that has a reference to the UI context.</param>
        /// <param name="hasAccess">Entity that requires access to UI context.</param>
        public static void RegisterForUIAccess(this IRegisterForUIAccess requiresAccess, IRequiresUIAccess hasAccess)
        {
            var cxt = hasAccess?.UIContext;
            if (cxt == null)
            {
                return;
            }

            requiresAccess?.RegisterForUIAccess(hasAccess?.UIContext);
        }

        /// <summary>
        /// Runs an action on the UI thread.
        /// </summary>
        /// <param name="context">The UI context.</param>
        /// <param name="action">The action to run.</param>
        /// <returns>Task to be awaited.</returns>
        public static async Task RunAsync(this IRequiresUIAccess context, Action action)
        {
#pragma warning disable 1998 // Required to force to Task overloaded method
            await context.UIContext.RunAsync(async () => action());
#pragma warning restore 1998
        }

        /// <summary>
        /// Runs an action on the UI thread.
        /// </summary>
        /// <param name="context">The UI context to run action on.</param>
        /// <param name="action">The action to run.</param>
        /// <returns>Task to be awaited.</returns>
        public static async Task RunAsync(this IRequiresUIAccess context, Func<Task> action)
        {
            var tsk = context?.UIContext?.RunAsync(action);
            if (tsk != null)
            {
                await tsk;
            }
            else
            {
                await action();
            }
        }

        /// <summary>
        /// Runs an action on the UI thread.
        /// </summary>
        /// <param name="context">The UI context to run the action on.</param>
        /// <param name="action">The action to run.</param>
        /// <returns>Task to await.</returns>
        public static async Task RunAsync(this IUIExecutionContext context, Action action)
        {
#pragma warning disable 1998 // Required to force to Task overloaded method
            await context.RunAsync(async () => action());
#pragma warning restore 1998
        }

        /// <summary>
        /// Runs an async task on UI thread.
        /// </summary>
        /// <param name="context">The UI context to run the task on.</param>
        /// <param name="action">The task to run.</param>
        /// <returns>Task to await.</returns>
        public static async Task RunAsync(this IUIExecutionContext context, Func<Task> action)
        {
            if (context == null)
            {
                "UI Context not defined, running action on current thread".LogMessage();
                await action();
                return;
            }

            if (context.IsRunningOnUIThread)
            {
                await action();
            }
            else
            {
                await context.RunOnUIThreadAsync(action);
            }
        }
    }
}