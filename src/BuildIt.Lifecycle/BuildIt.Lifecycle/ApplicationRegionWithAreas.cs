using BuildIt.Lifecycle.Interfaces;

namespace BuildIt.Lifecycle.Regions
{
    /// <summary>
    /// Application region that has areas for navigation
    /// </summary>
    public abstract class ApplicationRegionWithAreas : StateAwareApplicationRegion, IHasRegionAreas
    {
        /// <summary>
        /// Gets the number of areas
        /// </summary>
        public abstract int AreaCount { get; }
    }
}