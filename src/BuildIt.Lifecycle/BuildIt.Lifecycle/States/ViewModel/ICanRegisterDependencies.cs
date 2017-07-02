using BuildIt.ServiceLocation;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public interface ICanRegisterDependencies
    {
        void RegisterDependencies(IDependencyContainer container);
    }
}