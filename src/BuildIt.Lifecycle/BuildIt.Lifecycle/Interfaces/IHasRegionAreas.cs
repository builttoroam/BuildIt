namespace BuildIt.Lifecycle.Interfaces
{
    /// <summary>
    /// Indicates how many areas a region has
    /// </summary>
    public interface IHasRegionAreas
    {
        /// <summary>
        /// Gets the number of areas a region has
        /// </summary>
        int AreaCount { get; }
    }
}