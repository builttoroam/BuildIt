using Autofac;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public class BaseViewModel:NotifyBase, ICanRegisterDependencies, IRegisterForUIAccess
    {
        public IUIExecutionContext UIContext { get; private set; }
        public virtual void RegisterForUIAccess(IUIExecutionContext context)
        {
            UIContext = context;
        }

        public virtual void RegisterDependencies(IContainer container)
        {
            
        }
    }
}
