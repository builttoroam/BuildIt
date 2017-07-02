using System.Threading.Tasks;
using BuildIt.Lifecycle.States;
using BuildIt.ServiceLocation;

namespace BuildIt.Lifecycle
{
    public abstract class RegionAwareBaseApplication<TStartupRegion> : BaseApplication, 
        IHasRegionManager
        where TStartupRegion  : IApplicationRegion
    {

        public IRegionManager RegionManager { get; }

        protected RegionAwareBaseApplication()
        {
            RegionManager = new RegionManager();
            RegionManager.DefineRegion<TStartupRegion>();
        }

        protected virtual void DefineApplicationRegions()
        {
            
        }

        protected override async Task CommenceStartup()
        {
            await base.CommenceStartup();

            DefineApplicationRegions();
        }


        protected override async Task CompleteStartup()
        {
            await base.CompleteStartup();

            RegionManager.CreateRegion<TStartupRegion>();
        }

        protected override async Task BuildCoreDependencies(IDependencyContainer container)
        {
            await base.BuildCoreDependencies(container);
            
            RegionManager?.RegisterDependencies(container);
        }
    }
}