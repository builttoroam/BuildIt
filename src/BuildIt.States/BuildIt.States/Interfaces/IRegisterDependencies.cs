using BuildIt.ServiceLocation;

namespace BuildIt.States.Interfaces
{
    public interface IRegisterDependencies
    {
        void RegisterDependencies(IDependencyContainer container);
    }
}