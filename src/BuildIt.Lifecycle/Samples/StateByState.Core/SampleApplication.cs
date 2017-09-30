using BuildIt;
using BuildIt.Lifecycle;
using System.Diagnostics;
using StateByState.Regions.Main;
using StateByState.Regions.Secondary;

namespace StateByState
{
    public class SampleApplication : RegionAwareBaseApplication<MainRegion>
    {
#if DEBUG

        public SampleApplication()
        {
            LogHelper.LogOutput = entry => Debug.WriteLine(entry);
        }

#endif

        protected override void DefineApplicationRegions()
        {
            RegionManager.DefineRegion<MainRegion>();
            RegionManager.DefineRegion<SecondaryRegion>();
        }

        //protected override void RegisterDependencies(IDependencyContainer builder)
        //{
        //    base.RegisterDependencies(builder);
        //}
    }
}