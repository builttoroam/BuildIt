using System;
using BuildIt.Lifecycle.States.ViewModel;
using BuildIt.States;
using BuildIt.States.Interfaces;

namespace BuildIt.Lifecycle
{
    public interface IRegionManager : IRegisterDependencies
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