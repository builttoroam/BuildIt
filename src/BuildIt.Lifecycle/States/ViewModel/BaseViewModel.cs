using Autofac;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public class BaseViewModel:NotifyBase, ICanRegisterDependencies, IRequiresUIAccess
    {
        public UIExecutionContext UIContext { get; } = new UIExecutionContext();

        public virtual void RegisterDependencies(IContainer container)
        {
            
        }
    }
}
