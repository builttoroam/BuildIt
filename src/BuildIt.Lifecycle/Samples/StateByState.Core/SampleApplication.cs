using System;
using System.Threading.Tasks;
using Autofac;
using BuildIt;
using BuildIt.Lifecycle;
using BuildIt.ServiceLocation;
using BuildIt.States;

namespace StateByState
{
    public class SampleApplication : RegionAwareBaseApplication<MainRegion>
    {
        protected override void DefineApplicationRegions()
        {
            RegionManager.DefineRegion<MainRegion>();
            RegionManager.DefineRegion<SecondaryApplication>();
        }

        protected override void RegisterDependencies(IDependencyContainer builder)
        {
           // builder.Register<BasicDebugLogger,ILogService>();

            base.RegisterDependencies(builder);
        }
    }
}