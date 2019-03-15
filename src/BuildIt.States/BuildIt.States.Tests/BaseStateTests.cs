using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildIt.States.Tests
{
    public abstract class BaseStateTests
    {
        [TestInitialize]
        public void TestInit()
        {
            var af = new Autofac.AutofacDependencyContainer(null);
            LogHelper.LogOutput = entry => { return; }; //Debug.Write(entry);
        }
    }
}