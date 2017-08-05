using System;
using System.Threading.Tasks;
using BuildIt.States.Interfaces;

namespace BuildIt.Lifecycle.Interfaces
{
    /// <summary>
    /// Defines an application region
    /// </summary>
    public interface IApplicationRegion: IRegisterDependencies, IRegisterForUIAccess
    {
        /// <summary>
        /// Event indicating that a region should be closed
        /// </summary>
        event EventHandler CloseRegion;

        event EventHandler<CancelEventArgs> ClosingRegion;

        /// <summary>
        /// Unique identifier for the region
        /// </summary>
        string RegionId { get; }

        /// <summary>
        /// Gets the region manager associated with this region
        /// </summary>
        IRegionManager Manager { get; }

        /// <summary>
        /// Requests that a region be closed
        /// </summary>
        /// <returns>Task to await</returns>
        Task<bool> RequestClose();

        /// <summary>
        /// Starts the region
        /// </summary>
        /// <param name="manager">The region manager associated with this region</param>
        /// <returns>Task to await</returns>
        Task Startup(IRegionManager manager);
    }
}