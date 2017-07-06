using System;

namespace BuildIt.ServiceLocation
{
    /// <summary>
    /// Interface for a dependency container that supports registering types and instances
    /// </summary>
    public interface IDependencyContainer
    {
        /// <summary>
        /// Start an update to the container
        /// </summary>
        /// <returns>Disposable object that will end the container update</returns>
        IDisposable StartUpdate();

        /// <summary>
        /// Ends updating the container
        /// </summary>
        void EndUpdate();

        /// <summary>
        /// Registers a type based on an interface it implements
        /// </summary>
        /// <typeparam name="TTypeToRegister"></typeparam>
        /// <typeparam name="TInterfaceTypeToRegisterAs"></typeparam>
        void Register<TTypeToRegister, TInterfaceTypeToRegisterAs>()
            where TTypeToRegister : TInterfaceTypeToRegisterAs;

        /// <summary>
        /// Registers a type
        /// </summary>
        /// <typeparam name="TTypeToRegister"></typeparam>
        void Register<TTypeToRegister>();

        /// <summary>
        /// Registers a type, using the type as a parameter
        /// </summary>
        /// <param name="typeToRegister">The type to register</param>
        void RegisterType(Type typeToRegister);
    }
}