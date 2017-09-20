using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace BuildIt.States.Tests
{
    public abstract class BaseStateTests
    {
        [TestInitialize]
        public void TestInit()
        {
            LogHelper.LogOutput = entry => Debug.Write(entry);
        }
    }
}