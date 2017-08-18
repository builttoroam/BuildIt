using BuildIt.Lifecycle.Interfaces;
using BuildIt.Lifecycle.Regions;
using BuildIt.ServiceLocation;
using System.Threading.Tasks;

namespace BuildIt.Lifecycle
{
    /// <summary>
    /// An application that has a region manager, used for defining
    /// application regions
    /// </summary>
    /// <typeparam name="TStartupRegion">The type of the startup region</typeparam>
    public abstract class RegionAwareBaseApplication<TStartupRegion>
        : BaseApplication, IHasRegionManager
        where TStartupRegion : IApplicationRegion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegionAwareBaseApplication{TStartupRegion}"/> class.
        /// </summary>
        protected RegionAwareBaseApplication()
        {
            RegionManager = new RegionManager();
            RegionManager.DefineRegion<TStartupRegion>();
        }

        /// <summary>
        /// Gets the region manager
        /// </summary>
        public IRegionManager RegionManager { get; }

        /// <summary>
        /// Overridable method where application regions can be defined
        /// </summary>
        protected virtual void DefineApplicationRegions()
        {
        }

        /// <summary>
        /// Commences the application
        /// </summary>
        /// <returns>Task to await </returns>
        protected override async Task CommenceStartup()
        {
            DefineApplicationRegions();

            await base.CommenceStartup();
        }

        /// <summary>
        /// Completes startup
        /// </summary>
        /// <returns>Task to await </returns>
        protected override async Task CompleteStartup()
        {
            await base.CompleteStartup();

            RegionManager.CreateRegion<TStartupRegion>();
        }

        /// <summary>
        /// Register internal dependencies
        /// </summary>
        /// <param name="container">The dependency container</param>
        /// <returns>Task to await</returns>
        protected override async Task BuildCoreDependencies(IDependencyContainer container)
        {
            await base.BuildCoreDependencies(container);

            RegionManager?.RegisterDependencies(container);
        }
    }
}