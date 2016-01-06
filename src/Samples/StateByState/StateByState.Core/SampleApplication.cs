using System;
using System.Threading.Tasks;
using Autofac;
using BuildIt;
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

        protected override void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<BasicDebugLogger>().As<ILogService>();

            base.RegisterDependencies(builder);
        }
    }
}