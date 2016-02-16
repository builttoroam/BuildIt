using Autofac;
using BuildIt.States;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public class BaseStateManagerViewModel : BaseViewModel, IHasStates
    {
        public IStateManager StateManager { get; } = new StateManager();

        public override void RegisterForUIAccess(IUIExecutionContext context)
        {
            base.RegisterForUIAccess(context);

            foreach (var stateGroup in StateManager.StateGroups)
            {
                (stateGroup.Value as IRegisterForUIAccess)?.RegisterForUIAccess(context);
            }
        }

        public override void RegisterDependencies(IContainer container)
        {
            base.RegisterDependencies(container);
            foreach (var stateGroup in StateManager.StateGroups)
            {
                (stateGroup.Value as ICanRegisterDependencies)?.RegisterDependencies(container);
            }
        }

    }
}