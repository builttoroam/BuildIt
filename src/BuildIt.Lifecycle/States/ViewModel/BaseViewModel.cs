using Autofac;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public class BaseViewModel:NotifyBase, ICanRegisterDependencies, IRegisterForUIAccess
    {
        public UIExecutionContext UIContext { get; } = new UIExecutionContext();
        public void RegisterForUIAccess(IRequiresUIAccess manager)
        {
            manager.UIContext.RunContext = UIContext.RunContext;
        }

        public virtual void RegisterDependencies(IContainer container)
        {
            
        }
    }
}
