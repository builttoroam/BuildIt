using BuildIt.ServiceLocation;
using System;
using System.Threading.Tasks;

namespace BuildIt.Lifecycle
{
    /// <summary>
    /// Base definition of an application in the lifecycle model
    /// </summary>
    public class BaseApplication
    {
        /// <summary>
        /// Gets the dependency container for registering dependencies
        /// </summary>
        protected IDependencyContainer DependencyContainer { get; private set; }

        /// <summary>
        /// The startup process
        /// </summary>
        /// <param name="container">The dependency container</param>
        /// <param name="buildDependencies">An action delegate to define additioanl dependencies</param>
        /// <returns>Task to await</returns>
        public async Task Startup(
            IDependencyContainer container,
            Action<IDependencyContainer> buildDependencies = null)
        {
            await CommenceStartup();

            using (container.StartUpdate())
            {
                // Build and application dependencies
                RegisterDependencies(container);

                buildDependencies?.Invoke(container);

                await BuildCoreDependencies(container);
            }

            await CompleteStartup();
        }

        /// <summary>
        /// Register additional dependencies
        /// </summary>
        /// <param name="build">The action to invoke to build dependencies</param>
        public virtual void RegisterAdditionalDependencies(Action<IDependencyContainer> build)
        {
            build?.Invoke(DependencyContainer);
        }

        /// <summary>
        /// Commences the startup process
        /// </summary>
        /// <returns>Task to await</returns>
#pragma warning disable 1998 // Async so it can be overridden
        protected virtual async Task CommenceStartup()
#pragma warning restore 1998
        {
        }

        /// <summary>
        /// Completes the startup process
        /// </summary>
        /// <returns>Task to await</returns>
#pragma warning disable 1998 // Async so it can be overridden
        protected virtual async Task CompleteStartup()
#pragma warning restore 1998
        {
        }

        /// <summary>
        /// Registers dependencies
        /// </summary>
        /// <param name="builder">The dependency container builder</param>
        protected virtual void RegisterDependencies(IDependencyContainer builder)
        {
        }

        /// <summary>
        /// Tracks the dependency container
        /// </summary>
        /// <param name="container">The dependency container</param>
        /// <returns>Task to await</returns>
#pragma warning disable 1998 // Async so it can be overridden
        protected virtual async Task BuildCoreDependencies(IDependencyContainer container)
#pragma warning restore 1998
        {
            DependencyContainer = container;
        }
    }
}
