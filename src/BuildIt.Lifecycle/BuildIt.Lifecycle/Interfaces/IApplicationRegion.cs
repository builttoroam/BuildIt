using BuildIt.States.Interfaces;
using System;
using System.Threading.Tasks;

namespace BuildIt.Lifecycle.Interfaces
{
    /// <summary>
    /// Defines an application region
    /// </summary>
    public interface IApplicationRegion : IRegisterDependencies, IRegisterForUIAccess
    {
        /// <summary>
        /// Event indicating that a region should be closed
        /// </summary>
        event EventHandler CloseRegion;

        /// <summary>
        /// Event indicating that the region is planning to close
        /// </summary>
        event EventHandler<CancelEventArgs> ClosingRegion;

        /// <summary>
        /// Gets a unique identifier for the region
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