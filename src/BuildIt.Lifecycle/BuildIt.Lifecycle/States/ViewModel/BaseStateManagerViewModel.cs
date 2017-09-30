using BuildIt.ServiceLocation;
using BuildIt.States;
using BuildIt.States.Interfaces;

namespace BuildIt.Lifecycle.States.ViewModel
{
    /// <summary>
    /// Base view model that exposes a state manager
    /// </summary>
    public class BaseStateManagerViewModel : BaseViewModel, IHasStates
    {
        /// <summary>
        /// Gets gets or sets the state manager for the view model
        /// </summary>
        public IStateManager StateManager { get; } = new StateManager();

        /// <summary>
        /// Registers the view model and nested entities for UI access
        /// </summary>
        /// <param name="context">The UI Context</param>
        public override void RegisterForUIAccess(IUIExecutionContext context)
        {
            base.RegisterForUIAccess(context);

            foreach (var stateGroup in StateManager.StateGroups)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global // Not helpful as IStateGroup instance can implement IRegisterForUIAccess
                (stateGroup.Value as IRegisterForUIAccess)?.RegisterForUIAccess(context);
            }
        }

        /// <summary>
        /// Registers nested dependencies for injection
        /// </summary>
        /// <param name="container">The container to register with</param>
        public override void RegisterDependencies(IDependencyContainer container)
        {
            base.RegisterDependencies(container);
            foreach (var stateGroup in StateManager.StateGroups)
            {
                stateGroup.Value?.RegisterDependencies(container);
            }
        }
    }
}