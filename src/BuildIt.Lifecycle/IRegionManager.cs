using System;
using BuildIt.Lifecycle.States.ViewModel;

namespace BuildIt.Lifecycle
{
    public interface IRegionManager : ICanRegisterDependencies
    {
        Action<IRegionManager, IApplicationRegion> RegionCreated { get; set; }
        Action<IRegionManager, IApplicationRegion> RegionIsClosing { get; set; }


        //event EventHandler<ParameterEventArgs<IApplicationRegion>> RegionCreated;
        //event EventHandler<ParameterEventArgs<IApplicationRegion>> RegionIsClosing;

        void DefineRegion<TRegion>() where TRegion : IApplicationRegion;

        TRegion CreateRegion<TRegion>() where TRegion : IApplicationRegion;

        TRegion RegionByType<TRegion>() where TRegion : IApplicationRegion;

        IApplicationRegion RegionById(string id);

        bool IsPrimaryRegion(IApplicationRegion region);
    }
}