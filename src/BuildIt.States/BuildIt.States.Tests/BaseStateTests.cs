using Autofac;
using BuildIt.Autofac;
using BuildIt.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildIt.States.Tests
{
    public abstract class BaseStateTests
    {
        [TestInitialize]
        public void TestInit()
        {
            var build = new ContainerBuilder();
            var container = build.Build();

            var csl = new AutofacServiceLocator(container);
            var afContainer = new AutofacDependencyContainer(container);
            using (afContainer.StartUpdate())
            {
                afContainer.Register<TestDebugLogger, ILogService>();
            }
            ServiceLocator.SetLocatorProvider(() => csl);
        }
    }
}