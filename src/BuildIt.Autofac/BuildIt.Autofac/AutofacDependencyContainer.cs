using System;
using Autofac;
using BuildIt.ServiceLocation;

namespace BuildIt.Autofac
{
    public class AutofacDependencyContainer : IDependencyContainer
    {
        private IContainer Container { get; }
        private int editCount = 0;
        private ContainerBuilder Builder { get; set; }
        private IDisposable Wrapper { get; set; }
        public AutofacDependencyContainer(IContainer container)
        {
            Container = container;
        }

        private class ContainerWrapper : IDisposable
        {
            IDependencyContainer Container { get; }
            public ContainerWrapper(IDependencyContainer container)
            {
                Container = container;
            }

            public void Dispose()
            {
                Container.EndUpdate();
            }
        }


        public IDisposable StartUpdate()
        {
            editCount++;
            if (editCount == 1)
            {
                Builder = new ContainerBuilder();
                Wrapper = new ContainerWrapper(this);
            }
            return Wrapper;
        }

        public void EndUpdate()
        {
            editCount--;
            if (editCount == 0)
            {
#pragma warning disable CS0618 // Type or member is obsolete - see TODO below
                // TODO: Refactor the way the DI container is built to avoid updating it
                Builder.Update(Container);
#pragma warning restore CS0618 // Type or member is obsolete
                Builder = null;
                Wrapper = null;
            }
        }

        public void Register<TTypeToRegister, TInterfaceTypeToRegisterAs>()
        {
            Builder.RegisterType<TTypeToRegister>().As<TInterfaceTypeToRegisterAs>();
        }

        public void Register<TTypeToRegister>()
        {
            Builder.RegisterType<TTypeToRegister>();
        }

        public void RegisterType(Type typeToRegister)
        {
            Builder.RegisterType(typeToRegister);
        }
    }
}