using Autofac;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public class BaseViewModel:NotifyBase, ICanRegisterDependencies
    {
        public UIContext UIContext { get; } = new UIContext();

        public virtual void RegisterDependencies(IContainer container)
        {
            
        }
    }
}
