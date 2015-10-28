using System;
using System.Threading.Tasks;
using Autofac;
using BuildIt.Lifecycle.States.ViewModel;

namespace BuildIt.Lifecycle.States
{
    public interface IApplicationRegion: ICanRegisterDependencies
    {
        string RegionId { get; set; }

        event EventHandler CloseRegion;

        Task RequestClose();


        Task Startup(IRegionManager manager);
    }
}