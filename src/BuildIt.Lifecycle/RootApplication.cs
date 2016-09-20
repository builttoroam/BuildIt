using BuildIt.ServiceLocation;
using BuildIt.States;
using System;
using System.Threading.Tasks;

namespace BuildIt.Lifecycle
{
    public class BaseApplication
    {
        public async Task Startup(ServiceLocatorProvider locatorProvider,
            IDependencyContainer container,
            Action<IDependencyContainer> buildDependencies = null)
        {
            await CommenceStartup();

            //            var builder = new ContainerBuilder();

            using (container.StartUpdate())
            {
                // Build and application dependencies
                RegisterDependencies(container);

                buildDependencies?.Invoke(container);

                //// Perform registrations and build the container.
                //var container = builder.Build();

                await BuildCoreDependencies(container);
            }

            // Set the service locator to an AutofacServiceLocator.
            //var csl = new AutofacServiceLocator(container);
            //ServiceLocator.SetLocatorProvider(() => csl);
            ServiceLocator.SetLocatorProvider(locatorProvider);

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

        protected virtual void RegisterDependencies(IDependencyContainer builder)
        {

        }

        protected IDependencyContainer DependencyContainer { get; private set; }
#pragma warning disable 1998 // Async so it can be overridden
        protected virtual async Task BuildCoreDependencies(IDependencyContainer container)
#pragma warning restore 1998
        {
            DependencyContainer = container;
        }

        public void RegisterAdditionalDependencies(Action<IDependencyContainer> build)
        {
            //var cb = new ContainerBuilder();
            build?.Invoke(DependencyContainer);
            //cb.Update(DependencyContainer);

        }

    }
}
