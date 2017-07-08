using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildIt.States.Tests
{
    [TestClass]
    public class StateGroupTests: BaseStateTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        // ReSharper disable once ObjectCreationAsStatement - Intentional
        public void TestInvalidStateGroupCreationNullName() => new StateGroup(null);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        // ReSharper disable once ObjectCreationAsStatement - Intentional
        public void TestInvalidStateGroupCreationEmptyStringName() => new StateGroup(String.Empty);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        // ReSharper disable once ObjectCreationAsStatement - Intentional
        public void TestInvalidEnumStateGroupNotEnum() => new EnumStateGroup<NotAnEnum>();


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidEnumStateGroupDefineStateWithIncorrectState()
        {
            var esg = new EnumStateGroup<Test1State>();
            esg.DefineState(Test1State.OnlyState+"invalid");
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestInvalidEnumStateGroupDefineStateWithData()
        {
            var esg = new EnumStateGroup<Test1State>();
            esg.DefineStateWithData<DummyStateData>(nameof(Test1State.OnlyState));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidEnumStateGroupDefineStateWithInvalidStateDefinition()
        {
            var esg = new EnumStateGroup<Test1State>();
            var sd = new StateDefinition("test");
            esg.DefineState(sd);
        }

        [TestMethod]
        public void TestEmptyStateGroup()
        {
            var groupName = "test";
            var sg = new StateGroup(groupName);
            Assert.AreEqual(groupName, sg.GroupName);
            Assert.AreEqual(0, sg.States.Count);
            Assert.IsTrue(string.IsNullOrEmpty(sg.CurrentStateName));
            Assert.IsNull(sg.CurrentStateDefinition);
            Assert.IsNull(sg.CurrentStateData);
            Assert.IsNull(sg.CurrentStateDataWrapper);

            var esg = new EnumStateGroup<TestEnumNoStates>();
            groupName = nameof(TestEnumNoStates);
            Assert.AreEqual(groupName, esg.GroupName);
            Assert.AreEqual(0, esg.States.Count);
            Assert.IsTrue(string.IsNullOrEmpty(esg.CurrentStateName));
            Assert.IsNull(esg.CurrentStateDefinition);
            Assert.IsNull(esg.CurrentStateData);
            Assert.IsNull(esg.CurrentStateDataWrapper);
            Assert.IsNull(esg.CurrentEnumStateDefinition);
        }

        [TestMethod]
        public async Task TestOneStateGroup()
        {
            var groupName = "test";
            var sg = new StateGroup(groupName);
            var stateName = "one";
            var sd = sg.DefineState(stateName);
            Assert.IsNotNull(sd);
            Assert.AreEqual(stateName,sd.StateName);
            Assert.IsTrue(string.IsNullOrEmpty(sg.CurrentStateName));
            Assert.IsNull(sg.CurrentStateDefinition);
            Assert.IsNull(sg.CurrentStateData);
            Assert.IsNull(sg.CurrentStateDataWrapper);
            await sg.ChangeTo(stateName);
            Assert.AreEqual(stateName, sg.CurrentStateName);
            Assert.AreEqual(sd, sg.CurrentStateDefinition);
            Assert.IsNull(sg.CurrentStateData);
            Assert.IsNull(sg.CurrentStateDataWrapper);


            var esg = new EnumStateGroup<Test1State>();
            var esd = esg.DefineEnumState(Test1State.OnlyState);
            Assert.IsNotNull(esd);
            Assert.AreEqual(nameof(Test1State.OnlyState), esd.StateName);
            Assert.IsTrue(string.IsNullOrEmpty(esg.CurrentStateName));
            Assert.IsNull(esg.CurrentStateDefinition);
            Assert.IsNull(esg.CurrentStateData);
            Assert.IsNull(esg.CurrentStateDataWrapper);
            await esg.ChangeTo(Test1State.OnlyState);
            Assert.AreEqual(nameof(Test1State.OnlyState), esg.CurrentStateName);
            Assert.AreEqual(Test1State.OnlyState,esg.CurrentEnumState);
            Assert.AreEqual(esd, esg.CurrentStateDefinition);
            Assert.AreEqual(esd, esg.CurrentEnumStateDefinition);
            Assert.IsNull(esg.CurrentStateData);
            Assert.IsNull(esg.CurrentStateDataWrapper);
        }

        public enum Test1State
        {
            Base,
            OnlyState
        }

        public enum TestEnumNoStates
        {
            Base
        }

        public struct NotAnEnum
        {

        }

        public class DummyStateData: NotifyBase
        {
            
        }
    }
}
