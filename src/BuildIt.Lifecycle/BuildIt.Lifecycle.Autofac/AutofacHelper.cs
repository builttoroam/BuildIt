using Autofac;
using BuildIt.Autofac;
using BuildIt.ServiceLocation;
using System;
using System.Threading.Tasks;

namespace BuildIt.Lifecycle
{
    /// <summary>
    /// Extension class to make it easy to setup Autofac with Lifecycle
    /// </summary>
    public static class AutofacHelper
    {
        /// <summary>
        /// Creates the Autofac container and registers dependencies via a callback
        /// </summary>
        /// <param name="application">The root Lifecycle application</param>
        /// <param name="buildDependencies">The dependency callback</param>
        /// <returns>Task to await</returns>
        public static async Task Startup(
            this BaseApplication application,
            Action<IDependencyContainer> buildDependencies = null)
        {
            var build = new ContainerBuilder();
            var container = build.Build();
            var csl = new AutofacServiceLocator(container);

            ServiceLocator.SetLocatorProvider(() => csl);

            await application.Startup(new AutofacDependencyContainer(container), buildDependencies);
        }
    }
}