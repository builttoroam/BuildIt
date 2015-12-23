using System;
using System.Threading.Tasks;

namespace BuildIt.Lifecycle
{
    public static class UIHelper
    {
        public static void RegisterForUIAccess(this IRegisterForUIAccess requiresAccess, IRequiresUIAccess hasAccess)
        {
            requiresAccess?.RegisterForUIAccess(hasAccess?.UIContext);
        }

        public static async Task RunAsync(this IRequiresUIAccess context, Action action)
        {
#pragma warning disable 1998 // Required to force to Task overloaded method
            await context.UIContext.RunAsync(async () => action());
#pragma warning restore 1998
        }

        public static async Task RunAsync(this IRequiresUIAccess context, Func<Task> action)
        {
            await context.UIContext.RunAsync(action);
        }

        public static async Task RunAsync(this IUIExecutionContext context, Action action)
        {
#pragma warning disable 1998 // Required to force to Task overloaded method
            await context.RunAsync(async () => action());
#pragma warning restore 1998
        }

        public static async Task RunAsync(this IUIExecutionContext context, Func<Task> action)
        {
            if (context == null)
            {
                "UI Context not defined, running action on current thread".Log();
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