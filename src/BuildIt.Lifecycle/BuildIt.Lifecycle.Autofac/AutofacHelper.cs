using Autofac;
using BuildIt.Autofac;
using BuildIt.States;
using System;
using System.Threading.Tasks;
using BuildIt.ServiceLocation;

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

            ServiceLocator.SetLocatorProvider(() => csl);

            await application.Startup( new AutofacDependencyContainer(container), buildDependencies);
        }
    }
}
