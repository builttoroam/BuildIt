using BuildIt.States.Interfaces;
using System;
using System.Collections.Generic;

namespace BuildIt.Lifecycle.Interfaces
{
    /// <summary>
    /// Defines a region manager
    /// </summary>
    public interface IRegionManager : IRegisterDependencies
    {
        /// <summary>
        /// Event for when a region is created
        /// </summary>
        event EventHandler<DualParameterEventArgs<IRegionManager, IApplicationRegion>> RegionCreated;

        /// <summary>
        /// Event for when a region is closed
        /// </summary>
        event EventHandler<DualParameterEventArgs<IRegionManager, IApplicationRegion>> RegionIsClosed;

        /// <summary>
        /// Gets the current regions
        /// </summary>
        IEnumerable<IApplicationRegion> CurrentRegions { get; }

        /// <summary>
        /// Define a possible region - types need to be defined
        /// so that they can be registered with the DI framework
        /// </summary>
        /// <typeparam name="TRegion">The type of the region</typeparam>
        void DefineRegion<TRegion>()
            where TRegion : IApplicationRegion;

        /// <summary>
        /// Create a region of a particular type
        /// </summary>
        /// <typeparam name="TRegion">The type of region to create</typeparam>
        /// <param name="newRegionId">The Id of the region to create (optional)</param>
        /// <returns>The newly created region</returns>
        TRegion CreateRegion<TRegion>(string newRegionId = null)
            where TRegion : IApplicationRegion;

        /// <summary>
        /// Retrieve the active region(s) of a specific type
        /// </summary>
        /// <typeparam name="TRegion">The region type to retrieve</typeparam>
        /// <returns>The active regions</returns>
        IEnumerable<TRegion> RegionByType<TRegion>()
            where TRegion : IApplicationRegion;

        /// <summary>
        /// Retrieve an active region by its Id
        /// </summary>
        /// <param name="id">The id of the region tor retrieve</param>
        /// <returns>The active region</returns>
        IApplicationRegion RegionById(string id);

        /// <summary>
        /// Indicates if a region is the primary application region
        /// </summary>
        /// <param name="region">The region to test</param>
        /// <returns>bool indicating whether the region is the primary application region</returns>
        bool IsPrimaryRegion(IApplicationRegion region);
    }
}