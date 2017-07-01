using Autofac;
using BuildIt.Autofac;
using BuildIt.States;
using System;
using System.Threading.Tasks;

namespace BuildIt.Lifecycle
{
    public static class AutofacHelper
    {
        public static async Task Startup(
            this BaseApplication application,
            Action<IDependencyContainer> buildDependencies = null)
        {
            var build = new ContainerBuilder();
            var container = build.Build();
            var csl = new AutofacServiceLocator(container);

            // TODO: Fix up once BuildIT.States.IDependencyContainer has been removed
            //await application.Startup(() => csl, new AutofacDependencyContainer(container), buildDependencies);
        }
    }
}
