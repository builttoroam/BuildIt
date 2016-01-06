using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Microsoft.Practices.ServiceLocation;

namespace BuildIt.Lifecycle
{
    public class BaseApplication
    {
        public async Task Startup(Action<ContainerBuilder> buildDependencies = null)
        {
            await CommenceStartup();

            var builder = new ContainerBuilder();

            // Build and application dependencies
            RegisterDependencies(builder);

            buildDependencies?.Invoke(builder);

            // Perform registrations and build the container.
            var container = builder.Build();

            await BuildCoreDependencies(container);

            // Set the service locator to an AutofacServiceLocator.
            var csl = new AutofacServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => csl);

            await CompleteStartup();
        }

#pragma warning disable 1998 // Async so it can be overridden
        protected virtual async Task CommenceStartup()
#pragma warning restore 1998
        {
        }

#pragma warning disable 1998 // Async so it can be overridden
        protected virtual async Task CompleteStartup()
#pragma warning restore 1998
        {
        }

        protected virtual void RegisterDependencies(ContainerBuilder builder)
        {
            
        }

        protected IContainer DependencyContainer { get; private set; }
#pragma warning disable 1998 // Async so it can be overridden
        protected virtual async Task BuildCoreDependencies(IContainer container)
#pragma warning restore 1998
        {
            DependencyContainer = container;
        }

        public void RegisterAdditionalDependencies(Action<ContainerBuilder> build)
        {
            var cb = new ContainerBuilder();
            build?.Invoke(cb);
            cb.Update(DependencyContainer);

        }

    }
}
