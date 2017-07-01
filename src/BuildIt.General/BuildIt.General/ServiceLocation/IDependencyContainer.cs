using System;

namespace BuildIt.ServiceLocation
{
    public interface IDependencyContainer
    {
        IDisposable StartUpdate();
        void EndUpdate();
        void Register<TTypeToRegister, TInterfaceTypeToRegisterAs>();

        void Register<TTypeToRegister>();

        void RegisterType(Type typeToRegister);
    }
}