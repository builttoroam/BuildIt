using BuildIt.States;
using BuildIt.States.Interfaces;

namespace BuildIt.Lifecycle.Regions
{
    /// <summary>
    /// 
    /// </summary>
    public class StateAwareApplicationRegion : ApplicationRegion, IHasStates
    {
        /// <summary>
        /// 
        /// </summary>
        public IStateManager StateManager { get; } = new StateManager();
    }
}