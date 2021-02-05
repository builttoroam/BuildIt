using BuildIt.ServiceLocation;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// State that can register dependencies.
    /// </summary>
    public interface IRegisterDependencies
    {
        /// <summary>
        /// Registers dependencies.
        /// </summary>
        /// <param name="container">The container to register dependencies into.</param>
        void RegisterDependencies(IDependencyContainer container);
    }
}