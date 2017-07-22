using System;
using System.Threading.Tasks;
using BuildIt.States.Typed;
using BuildIt.States.Typed.Enum;
using BuildIt.States.Typed.String;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BuildIt.States.Tests
{
    [TestClass]
    public class CustomStateTypeTests : BaseStateTests
    {
        private class CustomStateType
        {
            public CustomStateType()
            {
            }

            public CustomStateType(Guid id)
            {
                Id = id;
            }
            public Guid Id { get; } = Guid.NewGuid();
        }

        private class CustomStateDefinition : TypedStateDefinition<CustomStateType>
        {
            public CustomStateDefinition()
            {

            }

            public override string StateName
            {
                get => State.Id.ToString();
//                set => State.Id = Guid.Parse(value);
            }
        }

        private class CustomStateGroupDefinition : TypedStateGroupDefinition<CustomStateType, CustomStateDefinition>
        {
            public CustomStateGroupDefinition()
            { }
        }

        private class CustomStateGroup : TypedStateGroup<CustomStateType, CustomStateDefinition, CustomStateGroupDefinition>
        {

        }

        [TestMethod]
        public async Task TestInvalidStateGroupCreationNullName()
        {
            var sg = new CustomStateGroup();
            Assert.IsNotNull(sg.TypedGroupDefinition);

            var s1 = sg.TypedGroupDefinition.DefineTypedState(new CustomStateType());
            var s2 = sg.TypedGroupDefinition.DefineTypedState(new CustomStateType());
            var s3 = sg.TypedGroupDefinition.DefineTypedState(new CustomStateType());

            Assert.IsNull(sg.CurrentState);
            Assert.AreEqual(3, sg.TypedGroupDefinition.States.Count);

            await sg.ChangeToStateByName(s1.StateName);
            Assert.AreSame(s1.State, sg.CurrentState);
            Assert.AreSame(s1,sg.CurrentStateDefinition);

            await sg.ChangeToStateByName(s2.StateName);
            Assert.AreSame(s2.State, sg.CurrentState);
            Assert.AreSame(s2, sg.CurrentStateDefinition);

            await sg.ChangeToStateByName(s3.StateName);
            Assert.AreSame(s3.State, sg.CurrentState);
            Assert.AreSame(s3, sg.CurrentStateDefinition);
        }
    }

    [TestClass]
    public class StateGroupTests : BaseStateTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        // ReSharper disable once ObjectCreationAsStatement - Intentional
        public void TestInvalidStateGroupCreationNullName() => new StateGroup(default(string));

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        // ReSharper disable once ObjectCreationAsStatement - Intentional
        public void TestInvalidStateGroupCreationEmptyStringName() => new StateGroup(String.Empty);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        // ReSharper disable once ObjectCreationAsStatement - Intentional
        public void TestInvalidEnumStateGroupNotEnum() => new EnumStateGroup<NotAnEnum>();


        // NR: Can't invoke this test as method has changed
        //[TestMethod]
        //[ExpectedException(typeof(ArgumentException))]
        //public void TestInvalidEnumStateGroupDefineStateWithIncorrectState()
        //{
        //    var esg = new EnumStateGroup<Test1State>();
        //    esg.DefineTypedState(Test1State.OnlyState+"invalid");
        //}

        // NR: Can't invoke this test as method has changed
        //[TestMethod]
        //[ExpectedException(typeof(NotSupportedException))]
        //public void TestInvalidEnumStateGroupDefineStateWithData()
        //{
        //    var esg = new EnumStateGroup<Test1State>();
        //    esg.DefineStateWithData<DummyStateData>(nameof(Test1State.OnlyState));
        //}

        // NR: Can't invoke this test as method has changed
        //[TestMethod]
        //[ExpectedException(typeof(ArgumentException))]
        //public void TestInvalidEnumStateGroupDefineStateWithInvalidStateDefinition()
        //{
        //    var esg = new EnumStateGroup<Test1State>();
        //    var sd = new TypedStateDefinition<Test1State>("test");
        //    esg.DefineTypedState(sd);
        //}

        [TestMethod]
        public void TestEmptyStateGroup()
        {
            var groupName = "test";
            var sg = new StateGroup(groupName);
            Assert.AreEqual(groupName, sg.GroupName);
            Assert.AreEqual(0, sg.GroupDefinition.States.Count);
            Assert.IsTrue(string.IsNullOrEmpty(sg.CurrentStateName));
            Assert.IsNull(sg.CurrentStateDefinition);
            Assert.IsNull(sg.CurrentStateData);
            Assert.IsNull(sg.CurrentStateDataWrapper);

            var esg = new EnumStateGroup<TestEnumNoStates>();
            groupName = nameof(TestEnumNoStates);
            Assert.AreEqual(groupName, esg.GroupName);
            Assert.AreEqual(0, esg.GroupDefinition.States.Count);
            Assert.IsTrue(string.IsNullOrEmpty(esg.CurrentStateName));
            Assert.IsNull(esg.CurrentStateDefinition);
            Assert.IsNull(esg.CurrentStateData);
            Assert.IsNull(esg.CurrentStateDataWrapper);
            Assert.IsNull(esg.CurrentTypedStateDefinition);
        }

        [TestMethod]
        public async Task TestOneStateGroup()
        {
            var groupName = "test";
            var sg = new StateGroup(groupName);
            var stateName = "one";
            var sd = sg.TypedGroupDefinition.DefineStateFromName(stateName);
            Assert.IsNotNull(sd);
            Assert.AreEqual(stateName, sd.StateName);
            Assert.IsTrue(string.IsNullOrEmpty(sg.CurrentStateName));
            Assert.IsNull(sg.CurrentStateDefinition);
            Assert.IsNull(sg.CurrentStateData);
            Assert.IsNull(sg.CurrentStateDataWrapper);
            await sg.ChangeToStateByName(stateName);
            Assert.AreEqual(stateName, sg.CurrentStateName);
            Assert.AreEqual(sd, sg.CurrentStateDefinition);
            Assert.IsNull(sg.CurrentStateData);
            Assert.IsNull(sg.CurrentStateDataWrapper);


            var esg = new EnumStateGroup<Test1State>();
            var esd = esg.TypedGroupDefinition.DefineTypedState(Test1State.OnlyState);
            Assert.IsNotNull(esd);
            Assert.AreEqual(nameof(Test1State.OnlyState), esd.StateName);
            Assert.IsTrue(string.IsNullOrEmpty(esg.CurrentStateName));
            Assert.IsNull(esg.CurrentStateDefinition);
            Assert.IsNull(esg.CurrentStateData);
            Assert.IsNull(esg.CurrentStateDataWrapper);
            await esg.ChangeToState(Test1State.OnlyState);
            Assert.AreEqual(nameof(Test1State.OnlyState), esg.CurrentStateName);
            Assert.AreEqual(Test1State.OnlyState, esg.CurrentState);
            Assert.AreEqual(esd, esg.CurrentStateDefinition);
            Assert.AreEqual(esd, esg.CurrentTypedStateDefinition);
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

        public class DummyStateData : NotifyBase
        {

        }
    }
}
