using BuildIt.States.Interfaces;

namespace BuildIt.Lifecycle.Regions
{
    /// <summary>
    /// Defines a region that contains only one area
    /// </summary>
    /// <typeparam name="TAreaState">The state type that controls navigation within the single area</typeparam>
    public class SingleAreaApplicationRegion<TAreaState> : ApplicationRegionWithAreas, ISingleAreaApplicationRegion
        where TAreaState : struct
    {
        /// <summary>
        /// Gets the state group that determines the state for the single area of the region
        /// </summary>
        public IStateGroup AreaStateGroup => StateManager.TypedStateGroup<TAreaState>();

        /// <summary>
        /// Gets the number of areas, which is 1
        /// </summary>
        public override int AreaCount => 1;
    }
}