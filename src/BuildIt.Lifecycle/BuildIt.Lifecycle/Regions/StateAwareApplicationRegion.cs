using BuildIt.Lifecycle.Interfaces;
using BuildIt.States;
using BuildIt.States.Interfaces;

namespace BuildIt.Lifecycle.Regions
{
    /// <summary>
    /// Application region that contains a state manager
    /// </summary>
    public class StateAwareApplicationRegion : ApplicationRegion, IStateAwareApplicationRegion
    {
        /// <summary>
        /// Gets the state manager that defines and controls the state of the region
        /// </summary>
        public IStateManager StateManager { get; } = new StateManager();
    }

    public interface IStateAwareApplicationRegion : IApplicationRegion, IHasStates
    {
    }
}