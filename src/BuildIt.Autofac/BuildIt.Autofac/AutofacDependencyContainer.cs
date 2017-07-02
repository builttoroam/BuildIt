using System;
using Autofac;
using BuildIt.ServiceLocation;

namespace BuildIt.Autofac
{
    /// <summary>
    /// An autofac dependency container that implements the BuildIt IDependencyContainer interface
    /// </summary>
    public class AutofacDependencyContainer : IDependencyContainer
    {
        private IContainer Container { get; }
        private int editCount = 0;
        private ContainerBuilder Builder { get; set; }
        private IDisposable Wrapper { get; set; }

        /// <summary>
        /// Creates new instance from an Autofac container
        /// </summary>
        /// <param name="container">The Autofac container to use</param>
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

        /// <summary>
        /// Start the process of updating the container
        /// </summary>
        /// <returns>Returns a disposable object that can be used to end the update</returns>
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

        /// <summary>
        /// Ends the updating of the container
        /// </summary>
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

        /// <summary>
        /// Register a type with the container
        /// </summary>
        /// <typeparam name="TTypeToRegister">The type to register</typeparam>
        /// <typeparam name="TInterfaceTypeToRegisterAs">The type/interface to register as</typeparam>
        public void Register<TTypeToRegister, TInterfaceTypeToRegisterAs>()
        {
            Builder.RegisterType<TTypeToRegister>().As<TInterfaceTypeToRegisterAs>();
        }

        /// <summary>
        /// Register a type with the container
        /// </summary>
        /// <typeparam name="TTypeToRegister"></typeparam>
        public void Register<TTypeToRegister>()
        {
            Builder.RegisterType<TTypeToRegister>();
        }

        /// <summary>
        /// Register a type with the container
        /// </summary>
        /// <param name="typeToRegister"></param>
        public void RegisterType(Type typeToRegister)
        {
            Builder.RegisterType(typeToRegister);
        }
    }
}