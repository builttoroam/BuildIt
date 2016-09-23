using Autofac;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public interface ICanRegisterDependencies
    {
        void RegisterDependencies(IContainer container);
    }
}