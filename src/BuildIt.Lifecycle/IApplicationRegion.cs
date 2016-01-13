using System;
using System.Threading.Tasks;
using BuildIt.Lifecycle.States.ViewModel;

namespace BuildIt.Lifecycle
{
    public interface IApplicationRegion: ICanRegisterDependencies, IRegisterForUIAccess
    {
        string RegionId { get; set; }

        event EventHandler CloseRegion;

        Task RequestClose();
        IRegionManager Manager { get; }
        Task Startup(IRegionManager manager);
    }
}