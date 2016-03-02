using System;
using System.Threading.Tasks;
using BuildIt.Lifecycle.States.ViewModel;
using BuildIt.States;

namespace BuildIt.Lifecycle
{
    public interface IApplicationRegion: IRegisterDependencies, IRegisterForUIAccess
    {
        string RegionId { get; set; }

        event EventHandler CloseRegion;

        Task RequestClose();
        IRegionManager Manager { get; }
        Task Startup(IRegionManager manager);
    }
}