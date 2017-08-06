using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
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
            Assert.AreSame(s1, sg.CurrentStateDefinition);

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
        // NR: Ideally this would be an ArgumentNullException but because the exception 
        // is raised within the constructor of a nested type (ie the GroupDefinition) it
        // is raised as a TargetInvocationException
        [ExpectedException(typeof(TargetInvocationException))]
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

        [TestMethod]
        public async Task TestStagesOfStateTransitions()
        {
            var steps = new List<int>();
            var stepStates = new List<Test3State>();

            var sm = new StateManager();
            sm.DefineState(Test3State.State1)
                .WhenChangedTo(cancel =>
                {
                    steps.Add(0);
                    stepStates.Add(sm.CurrentState<Test3State>());
                })
                .WhenAboutToChangeFrom(cancel =>
                {
                    steps.Add(1);
                    stepStates.Add(sm.CurrentState<Test3State>());
                })
                .WhenChangingFrom(cancel =>
                {
                    steps.Add(2);
                    stepStates.Add(sm.CurrentState<Test3State>());
                })
                .WhenChangedFrom(cancel =>
                {
                    steps.Add(3);
                    stepStates.Add(sm.CurrentState<Test3State>());
                })
                .DefineState(Test3State.State2)
                .WhenChangedTo(cancel =>
                {
                    steps.Add(4);
                    stepStates.Add(sm.CurrentState<Test3State>());
                })
                .WhenAboutToChangeFrom(cancel =>
                {
                    steps.Add(5);
                    stepStates.Add(sm.CurrentState<Test3State>());
                })
                .WhenChangingFrom(cancel =>
                {
                    steps.Add(6);
                    stepStates.Add(sm.CurrentState<Test3State>());
                })
                .WhenChangedFrom(cancel =>
                {
                    steps.Add(7);
                    stepStates.Add(sm.CurrentState<Test3State>());
                })
                .DefineState(Test3State.State3)
                .WhenChangedTo(cancel =>
                {
                    steps.Add(8);
                    stepStates.Add(sm.CurrentState<Test3State>());
                });

            await sm.GoToState(Test3State.State1);
            await sm.GoToState(Test3State.State2);
            await sm.GoToState(Test3State.State3);
            Assert.AreEqual(9, steps.Count);
            Assert.AreEqual(9, stepStates.Count);
            for (var i = 0; i < steps.Count; i++)
            {
                Assert.AreEqual(i, steps[i]);
                switch (i)
                {
                    case 0:
                    case 1:
                    case 2:
                        Assert.AreEqual(Test3State.State1, stepStates[i]);
                        break;
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        Assert.AreEqual(Test3State.State2, stepStates[i]);
                        break;
                    case 7:
                    case 8:
                        Assert.AreEqual(Test3State.State3, stepStates[i]);
                        break;
                    default:
                        Assert.Fail("Invalid step");
                        break;
                }
            }

        }

        [TestMethod]
        public async Task TestCancelGroup()
        {
            var sm = new StateManager();
            sm.Group<Test2State>()
                .DefineState(Test2State.State1)
                .WhenAboutToChangeFrom(async cancel =>
                {
                    await Task.Delay(30000, cancel.CancelToken);
                })
                .DefineState(Test2State.State2);

            var cancelT = new CancellationTokenSource();
            await sm.GoToState(Test2State.State1);
            Assert.AreEqual(Test2State.State1, sm.CurrentState<Test2State>());
            var waiter = sm.GoToState(Test2State.State2, false, cancelT.Token);
            cancelT.Cancel();
            await waiter;
            Assert.AreEqual(Test2State.State1, sm.CurrentState<Test2State>());

            sm = new StateManager();
            sm.Group<Test2State>()
                .DefineState(Test2State.State1)
                .WhenChangingFrom(async cancel =>
                {
                    await Task.Delay(30000, cancel);
                })
                .DefineState(Test2State.State2);

            cancelT = new CancellationTokenSource();
            await sm.GoToState(Test2State.State1);
            Assert.AreEqual(Test2State.State1, sm.CurrentState<Test2State>());
            waiter = sm.GoToState(Test2State.State2, false, cancelT.Token);
            cancelT.Cancel();
            await waiter;
            Assert.AreEqual(Test2State.State2, sm.CurrentState<Test2State>());
        }
        public enum Test3State
        {
            Base,
            State1,
            State2,
            State3
        }
        public enum Test2State
        {
            Base,
            State1,
            State2
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
