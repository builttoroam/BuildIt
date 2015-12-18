using System;
using System.Threading.Tasks;
using BuildIt.Lifecycle;

namespace StateByState
{
    public class SampleApplication : RegionAwareBaseApplication<MainRegion>
    {
        protected override void DefineApplicationRegions()
        {
            RegionManager.DefineRegion<MainRegion>();
            RegionManager.DefineRegion<SecondaryApplication>();
        }
    }
}