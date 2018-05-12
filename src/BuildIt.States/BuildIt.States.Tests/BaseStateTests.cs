using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace BuildIt.States.Tests
{
    public abstract class BaseStateTests
    {
        [TestInitialize]
        public void TestInit()
        {
            var af = new Autofac.AutofacDependencyContainer(null);
            LogHelper.LogOutput = entry => Debug.Write(entry);
        }
    }
}