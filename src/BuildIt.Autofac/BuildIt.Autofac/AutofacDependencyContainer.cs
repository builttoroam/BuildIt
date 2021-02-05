using Autofac;
using BuildIt.ServiceLocation;
using System;

namespace BuildIt.Autofac
{
    /// <summary>
    /// An autofac dependency container that implements the BuildIt IDependencyContainer interface.
    /// </summary>
    public class AutofacDependencyContainer : IDependencyContainer
    {
        private int editCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacDependencyContainer"/> class.
        /// </summary>
        /// <param name="container">The Autofac container to use.</param>
        public AutofacDependencyContainer(IContainer container)
        {
            Container = container;
        }

        private IContainer Container { get; }

        private ContainerBuilder Builder { get; set; }

        private IDisposable Wrapper { get; set; }

        /// <summary>
        /// Start the process of updating the container.
        /// </summary>
        /// <returns>Returns a disposable object that can be used to end the update.</returns>
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
        /// Ends the updating of the container.
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
        /// Register a type with the container.
        /// </summary>
        /// <typeparam name="TTypeToRegister">The type to register.</typeparam>
        /// <typeparam name="TInterfaceTypeToRegisterAs">The type/interface to register as.</typeparam>
        public void Register<TTypeToRegister, TInterfaceTypeToRegisterAs>()
            where TTypeToRegister : TInterfaceTypeToRegisterAs
        {
            Builder.RegisterType<TTypeToRegister>().As<TInterfaceTypeToRegisterAs>();
        }

        /// <summary>
        /// Register a type with the container.
        /// </summary>
        /// <typeparam name="TTypeToRegister">The type to register.</typeparam>
        public void Register<TTypeToRegister>()
        {
            Builder.RegisterType<TTypeToRegister>();
        }

        /// <summary>
        /// Register a type with the container.
        /// </summary>
        /// <param name="typeToRegister">The type to register.</param>
        public void RegisterType(Type typeToRegister)
        {
            Builder.RegisterType(typeToRegister);
        }

        private class ContainerWrapper : IDisposable
        {
            public ContainerWrapper(IDependencyContainer container)
            {
                Container = container;
            }

            private IDependencyContainer Container { get; }

            public void Dispose()
            {
                Container.EndUpdate();
            }
        }
    }
}