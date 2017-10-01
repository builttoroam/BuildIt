using BuildIt.States.Interfaces;

namespace BuildIt.Lifecycle.Regions
{
    /// <summary>
    /// Identified a region with a single navigation area
    /// </summary>
    public interface ISingleAreaApplicationRegion : IStateAwareApplicationRegion
    {
        /// <summary>
        /// Gets the state group that will control navigation within the area
        /// </summary>
        IStateGroup AreaStateGroup { get; }
    }
}