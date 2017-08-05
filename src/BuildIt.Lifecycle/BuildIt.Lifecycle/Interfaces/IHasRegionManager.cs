namespace BuildIt.Lifecycle.Interfaces
{
    /// <summary>
    /// Indicates that an entity has a region manager
    /// </summary>
    public interface IHasRegionManager
    {
        /// <summary>
        /// Gets the associated region manager
        /// </summary>
        IRegionManager RegionManager { get; }
    }
}