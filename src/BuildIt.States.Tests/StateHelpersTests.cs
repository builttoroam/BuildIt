using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace BuildIt.States.Tests
{
    [TestClass]
    public class StateHelpersTests
    {
        public enum TestStates
        {
            Base,
            State1,
            State2,
            State3
        }

        public enum MoreStates
        {
            Base,
            State5,
            State6,
            State7
        }


        [TestMethod]
        public void TestGroup_IStateManager()
        {
            var sm = new StateManager();
            Assert.AreEqual(0,sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            Assert.AreEqual(1, sm.StateGroups.Count);
            var grp = sm.StateGroups[typeof (TestStates)] as StateGroup<TestStates>;
            Assert.IsNotNull(grp);
            Assert.IsInstanceOfType(builder,typeof(IStateGroupBuilder<TestStates>));
        }

        [TestMethod]
        public void TestGroup_IStateBuilder()
        {
            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            Assert.AreEqual(1, sm.StateGroups.Count);
            var builder2 = sm.Group<MoreStates>();
            Assert.AreEqual(2, sm.StateGroups.Count);
            Assert.AreNotEqual(builder,builder2);

            var grp = sm.StateGroups[typeof(MoreStates)] as StateGroup<MoreStates>;
            Assert.IsNotNull(grp);
            Assert.IsInstanceOfType(builder2, typeof(IStateGroupBuilder<MoreStates>));
        }


        [TestMethod]
        public void TestWithHistory()
        {
            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>()
                            .WithHistory();
            Assert.AreEqual(1, sm.StateGroups.Count);
            var grp = sm.StateGroups[typeof(TestStates)] as StateGroup<TestStates>;
            Assert.AreEqual(true,grp.TrackHistory);
        }
    }
}
