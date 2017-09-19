using BuildIt.ServiceLocation;
//using BuildIt.Autofac;
//using BuildIt.ServiceLocation;
using BuildIt.States.Completion;
using BuildIt.States.Interfaces;
using BuildIt.States.Interfaces.Builder;
using BuildIt.States.Interfaces.StateData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BuildIt.Logging;
using BuildIt.Logging.Filters;
using BuildIt.States.Typed.String;

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

        public class State2Data : NotifyBase, IInitialiseWithData<string>, IInitialiseWithData<int>
        {
            public string InitValue1 { get; set; }

            public Task InitialiseWithData(string data, CancellationToken cancelToken)
            {
                InitValue1 = data;
                return Task.CompletedTask;
            }

            public Task InitialiseWithData(int data, CancellationToken cancelToken)
            {
                InitValue1 = $"Init {data}";
                return Task.CompletedTask;
            }
        }
        public class State1Data : NotifyBase,
            ICompletion<DefaultCompletion>,
            ICompletion<TestCompletion>,
            ICompletionWithData<TestCompletion, int>
        {
            private event EventHandler<CompletionEventArgs<TestCompletion>> TestComplete;


            public event EventHandler<CompletionWithDataEventArgs<TestCompletion, int>> CompleteWithData;
            public event EventHandler<CompletionEventArgs<DefaultCompletion>> Complete;

            public event EventHandler CustomEvent1;
            public event EventHandler<ParameterEventArgs<bool>> CustomEvent2;

            public void RaiseCustomEvent1()
            {
                CustomEvent1?.Invoke(this, EventArgs.Empty);
            }

            public void RaiseCustomEvent2(bool val)
            {
                CustomEvent2?.Invoke(this, val);
            }

            public void RaiseComplete()
            {
                Complete?.Invoke(this,
                    new CompletionEventArgs<DefaultCompletion> { Completion = DefaultCompletion.Complete });
            }

            public bool HasCustomEvent1Handlers => CustomEvent1 != null;
            public bool HasCustomEvent2Handlers => CustomEvent2 != null;

            public void RaiseTestComplete(TestCompletion completion)
            {
                TestComplete?.Invoke(this, new CompletionEventArgs<TestCompletion> { Completion = completion });
            }

            public void RaiseTestCompleteWithData(TestCompletion completion)
            {
                CompleteWithData?.Invoke(this, new CompletionWithDataEventArgs<TestCompletion, int> { Completion = completion, Data = TestBoolValue ? 1 : 0 });
            }

            event EventHandler<CompletionEventArgs<TestCompletion>> ICompletion<TestCompletion>.Complete
            {
                add => TestComplete += value;
                remove => TestComplete -= value;
            }

            public bool TestBoolValue { get; set; }
        }

        public enum TestCompletion
        {
            Base,
            Complete1,
            Complete2,
            Complete3
        }

        public enum MoreStates
        {
            Base,
            State5,
            State6,
            State7
        }

        private IDependencyContainer Container { get; set; }

        [TestInitialize]
        public void TestInit()
        {
            LogHelper.LogOutput = entry => Debug.Write(entry);
        }


        [TestMethod]
        public void TestNullBuilder()
        {
            var stateManager = default(IStateManager);
            Assert.IsNull(stateManager.Group<TestStates>());
            var stateBuilder = default(IStateBuilder);
            Assert.IsNull(stateBuilder.Group<TestStates>());
            var stateGroupBuilder = default(IStateGroupBuilder<TestStates>);
            Assert.IsNull(stateGroupBuilder.WithHistory());
            Assert.IsNull(stateGroupBuilder.DefineAllStates());
            Assert.IsNull(stateGroupBuilder.DefineState(TestStates.State1));
            var stateDefinitionBuilder = default(IStateDefinitionBuilder<TestStates>);
            Assert.IsNull(stateDefinitionBuilder.AddTrigger(new StateTriggerBase()));

            var stateDefinitionWithDataBuilder = default(IStateDefinitionWithDataBuilder<TestStates, State1Data>);

            Assert.IsNull(stateDefinitionBuilder.OnComplete(TestCompletion.Complete1));
            Assert.IsNull(stateDefinitionBuilder.OnDefaultComplete());

            Assert.IsNull(stateDefinitionWithDataBuilder.OnComplete(TestCompletion.Complete2));
            Assert.IsNull(stateDefinitionWithDataBuilder.OnDefaultComplete());


            Assert.IsNull(stateDefinitionBuilder.OnDefaultCompleteWithData(() => false));
            Assert.IsNull(stateDefinitionBuilder.OnCompleteWithData(TestCompletion.Complete1, () => false));

            Assert.IsNull(stateDefinitionWithDataBuilder.OnCompleteWithData(TestCompletion.Complete2, () => false));
            Assert.IsNull(stateDefinitionWithDataBuilder.OnDefaultCompleteWithData(() => false));
        }


        [TestMethod]
        public void TestGroup_IStateManager()
        {
            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            Assert.AreEqual(1, sm.StateGroups.Count);
            var grp = sm.TypedStateGroup<TestStates>();//StateGroups[typeof(TestStates)] as EnumStateGroup<TestStates>;
            Assert.IsNotNull(grp);
            Assert.IsInstanceOfType(builder, typeof(IStateGroupBuilder<TestStates>));
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
            Assert.AreNotEqual(builder, builder2);

            var grp = sm.TypedStateGroup<MoreStates>();//StateGroups[typeof(MoreStates)] as EnumStateGroup<MoreStates>;
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
            var grp = sm.TypedStateGroup<TestStates>();//StateGroups[typeof(TestStates)] as EnumStateGroup<TestStates>;
            Assert.AreEqual(true, grp.TrackHistory);
        }


        [TestMethod]
        public void TestDefineAllStates()
        {
            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            var grp = sm.TypedStateGroup<TestStates>();//StateGroups[typeof(TestStates)] as EnumStateGroup<TestStates>;
            Assert.AreEqual(0, grp.GroupDefinition.States.Count);
            builder = builder.DefineAllStates();

            // Note: There is no state defined for the first, or default, value of an enum
            Assert.AreEqual(
                typeof(TestStates).GetRuntimeFields().Count(f => (f.Attributes & FieldAttributes.Literal) > 0) - 1,
                grp.GroupDefinition.States.Count);
        }

        [TestMethod]
        public void TestDefineState()
        {
            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            var grp = sm.TypedStateGroup<TestStates>();//StateGroups[typeof(TestStates)] as EnumStateGroup<TestStates>;
            Assert.AreEqual(0, grp.GroupDefinition.States.Count);
            builder.DefineState(TestStates.Base);
            Assert.AreEqual(0, grp.GroupDefinition.States.Count);
            builder.DefineState(TestStates.State1);
            Assert.AreEqual(1, grp.GroupDefinition.States.Count);
            var state = grp.GroupDefinition.States[TestStates.State1 + ""];
            builder.DefineState(TestStates.State1);
            Assert.AreEqual(1, grp.GroupDefinition.States.Count);
            var newstate = grp.GroupDefinition.States[TestStates.State1 + ""];
            Assert.AreEqual(state, newstate);

        }


        [TestMethod]
        public void TestAddTrigger()
        {
            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            var grp = sm.TypedStateGroup<TestStates>();//StateGroups[typeof(TestStates)] as EnumStateGroup<TestStates>;

            var st = builder.DefineState(TestStates.State1);
            Assert.AreEqual(0, st.State.Triggers.Count);
            var stt = st.AddTrigger(null);
            Assert.AreEqual(st, stt);
            Assert.AreEqual(0, st.State.Triggers.Count);

            var trigger = new StateTriggerBase();
            st.AddTrigger(trigger);
            var trigger2 = new StateTriggerBase();
            builder.DefineState(TestStates.State2).AddTrigger(trigger2);


            var current = sm.CurrentState<TestStates>();
            Assert.AreEqual(current, TestStates.Base);
            trigger.UpdateIsActive(true);
            current = sm.CurrentState<TestStates>();
            Assert.AreEqual(current, TestStates.State1);
            trigger2.UpdateIsActive(true); // Since state1 still active, this won't change current state
            current = sm.CurrentState<TestStates>();
            Assert.AreEqual(current, TestStates.State1);
            trigger.UpdateIsActive(false);
            current = sm.CurrentState<TestStates>();
            Assert.AreEqual(current, TestStates.State2);
            trigger.UpdateIsActive(true); // Since state2 still active, this won't change current state
            current = sm.CurrentState<TestStates>();
            Assert.AreEqual(current, TestStates.State2);
            trigger2.UpdateIsActive(false);
            current = sm.CurrentState<TestStates>();
            Assert.AreEqual(current, TestStates.State1);
        }

        [TestMethod]
        public void TestOnComplete()
        {
            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            var grp = sm.TypedStateGroup<TestStates>();//StateGroups[typeof(TestStates)] as EnumStateGroup<TestStates>;

            var st = builder.DefineState(TestStates.State1).OnComplete(TestCompletion.Complete1);
            Assert.IsInstanceOfType(st, typeof(IStateCompletionBuilder<TestStates, TestCompletion>));

            Assert.IsNotNull(st.StateManager);
            Assert.IsNotNull(st.StateGroup);
            Assert.IsNotNull(st.State);
            Assert.AreEqual(TestCompletion.Complete1, st.Completion);
        }

        [TestMethod]
        public void TestOnDefaultComplete()
        {
            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            var grp = sm.TypedStateGroup<TestStates>();//StateGroups[typeof(TestStates)] as EnumStateGroup<TestStates>;

            var st = builder.DefineState(TestStates.State1).OnDefaultComplete();
            Assert.IsInstanceOfType(st, typeof(IStateCompletionBuilder<TestStates, DefaultCompletion>));

            Assert.IsNotNull(st.StateManager);
            Assert.IsNotNull(st.StateGroup);
            Assert.IsNotNull(st.State);
            Assert.AreEqual(DefaultCompletion.Complete, st.Completion);
        }

        [TestMethod]
        public void TestOnComplete_StateWithData()
        {
            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            var grp = sm.TypedStateGroup<TestStates>();//StateGroups[typeof(TestStates)] as EnumStateGroup<TestStates>;

            var st =
                builder.DefineStateWithData<TestStates, State1Data>(TestStates.State1)
                    .OnComplete(TestCompletion.Complete1);
            Assert.IsInstanceOfType(st, typeof(IStateWithDataCompletionBuilder<TestStates, State1Data, TestCompletion>));

            Assert.IsNotNull(st.StateManager);
            Assert.IsNotNull(st.StateGroup);
            Assert.IsNotNull(st.State);
            Assert.IsNotNull(st.StateDataWrapper);
            Assert.AreEqual(TestCompletion.Complete1, st.Completion);
        }

        [TestMethod]
        public void TestOnDefaultComplete_StateWithData()
        {
            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            var grp = sm.TypedStateGroup<TestStates>();//StateGroups[typeof(TestStates)] as EnumStateGroup<TestStates>;

            var st = builder.DefineStateWithData<TestStates, State1Data>(TestStates.State1).OnDefaultComplete();
            Assert.IsInstanceOfType(st,
                typeof(IStateWithDataCompletionBuilder<TestStates, State1Data, DefaultCompletion>));

            Assert.IsNotNull(st.StateManager);
            Assert.IsNotNull(st.StateGroup);
            Assert.IsNotNull(st.State);
            Assert.IsNotNull(st.StateDataWrapper);
            Assert.AreEqual(DefaultCompletion.Complete, st.Completion);
        }

        [TestMethod]
        public void TestOnCompleteWithData()
        {
            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            var grp = sm.TypedStateGroup<TestStates>();//StateGroups[typeof(TestStates)] as EnumStateGroup<TestStates>;
            var complete = false;
            var st = builder.DefineState(TestStates.State1).OnCompleteWithData(TestCompletion.Complete1, () => complete);
            Assert.IsInstanceOfType(st, typeof(IStateCompletionWithDataBuilder<TestStates, TestCompletion, bool>));

            Assert.IsNotNull(st.StateManager);
            Assert.IsNotNull(st.StateGroup);
            Assert.IsNotNull(st.State);
            Assert.AreEqual(TestCompletion.Complete1, st.Completion);
            Assert.IsNotNull(st.Data);
            Assert.AreEqual(false, st.Data());
            complete = true;
            Assert.AreEqual(true, st.Data());
        }

        [TestMethod]
        public void TestOnDefaultCompleteWithData()
        {
            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            var grp = sm.TypedStateGroup<TestStates>();//StateGroups[typeof(TestStates)] as EnumStateGroup<TestStates>;
            var complete = false;
            var st = builder.DefineState(TestStates.State1).OnDefaultCompleteWithData(() => complete);
            Assert.IsInstanceOfType(st, typeof(IStateCompletionWithDataBuilder<TestStates, DefaultCompletion, bool>));

            Assert.IsNotNull(st.StateManager);
            Assert.IsNotNull(st.StateGroup);
            Assert.IsNotNull(st.State);
            Assert.AreEqual(DefaultCompletion.Complete, st.Completion);
            Assert.IsNotNull(st.Data);
            Assert.AreEqual(false, st.Data());
            complete = true;
            Assert.AreEqual(true, st.Data());

        }


        [TestMethod]
        public void TestOnCompleteWithData_StateWithData()
        {
            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            var grp = sm.TypedStateGroup<TestStates>();//StateGroups[typeof(TestStates)] as EnumStateGroup<TestStates>;
            var st =
                builder.DefineStateWithData<TestStates, State1Data>(TestStates.State1)
                    .OnCompleteWithData(TestCompletion.Complete1, (sd) => sd.TestBoolValue);
            Assert.IsInstanceOfType(st,
                typeof(IStateWithDataCompletionWithDataBuilder<TestStates, State1Data, TestCompletion, bool>));

            Assert.IsNotNull(st.StateManager);
            Assert.IsNotNull(st.StateGroup);
            Assert.IsNotNull(st.State);
            Assert.IsNotNull(st.StateDataWrapper);
            Assert.AreEqual(TestCompletion.Complete1, st.Completion);
            Assert.IsNotNull(st.Data);
            var vm = new State1Data
            {
                TestBoolValue = false
            };
            Assert.AreEqual(false, st.Data(vm));
            vm.TestBoolValue = true;
            Assert.AreEqual(true, st.Data(vm));
        }

        [TestMethod]
        public void TestOnDefaultCompleteWithData_StateWithData()
        {

            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            var grp = sm.TypedStateGroup<TestStates>();//StateGroups[typeof(TestStates)] as EnumStateGroup<TestStates>;

            var st =
                builder.DefineStateWithData<TestStates, State1Data>(TestStates.State1)
                    .OnDefaultCompleteWithData((sd) => sd.TestBoolValue);
            Assert.IsInstanceOfType(st,
                typeof(IStateWithDataCompletionWithDataBuilder<TestStates, State1Data, DefaultCompletion, bool>));

            Assert.IsNotNull(st.StateManager);
            Assert.IsNotNull(st.StateGroup);
            Assert.IsNotNull(st.State);
            Assert.IsNotNull(st.StateDataWrapper);
            Assert.AreEqual(DefaultCompletion.Complete, st.Completion);
            var vm = new State1Data { TestBoolValue = false };
            Assert.AreEqual(false, st.Data(vm));
            vm.TestBoolValue = true;
            Assert.AreEqual(true, st.Data(vm));

        }


        [TestMethod]
        public async Task TestTarget()
        {
            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            var grp = sm.TypedStateGroup<TestStates>();//StateGroups[typeof(TestStates)] as EnumStateGroup<TestStates>;

            var sd = new State1Data();

            builder.DefineState(TestStates.State1)
                .Target(sd)
                .Change(s => s.TestBoolValue)
                .ToValue(true);

            builder.DefineState(TestStates.State2)
                .Target(sd)
                .Change(s => s.TestBoolValue, (s, x) => s.TestBoolValue = x)
                .ToValue(false);
            Assert.IsFalse(sd.TestBoolValue);
            await sm.GoToState(TestStates.State1);
            Assert.IsTrue(sd.TestBoolValue);
            await sm.GoToState(TestStates.State2);
            Assert.IsFalse(sd.TestBoolValue);

        }


        [TestMethod]
        public async Task TestOnEvent()
        {
            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            var grp = sm.TypedStateGroup<TestStates>();//StateGroups[typeof(TestStates)] as EnumStateGroup<TestStates>;

            var sd = new State1Data();

            builder.DefineStateWithData<TestStates, State1Data>(TestStates.State1)
                .OnEvent((x, e) => x.CustomEvent1 += e,
                    (x, e) => x.CustomEvent1 -= e)
                .ChangeState(TestStates.State2);
            builder.DefineState(TestStates.State2);

            grp.RegisterDependencies(Container);

            Assert.AreEqual(TestStates.Base, sm.CurrentState<TestStates>());
            var data = (sm.TypedStateGroup<TestStates>()
            //StateGroups[typeof(TestStates)] 
            ).CurrentStateData as State1Data;
            Assert.IsNull(data);

            await sm.GoToState(TestStates.State1);
            Assert.AreEqual(TestStates.State1, sm.CurrentState<TestStates>());
            data = (sm.TypedStateGroup<TestStates>()
                //StateGroups[typeof(TestStates)] 
                ).CurrentStateData as State1Data;
            Assert.IsNotNull(data);
            Assert.IsTrue(data.HasCustomEvent1Handlers);


            data.RaiseCustomEvent1();
            Assert.AreEqual(TestStates.State2, sm.CurrentState<TestStates>());
            var newdata = (sm.TypedStateGroup<TestStates>()
            //StateGroups[typeof(TestStates)] 
            ).CurrentStateData as State1Data;
            Assert.IsNull(newdata);
            Assert.IsFalse(data.HasCustomEvent1Handlers);
        }


        [TestMethod]
        public async Task TestOnCompleteChangeState()
        {
            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            var grp = sm.TypedStateGroup<TestStates>();//StateGroups[typeof(TestStates)] as EnumStateGroup<TestStates>;

            var sd = new State1Data();

            builder.DefineStateWithData<TestStates, State1Data>(TestStates.State1)
                .OnComplete(TestCompletion.Complete1)
                .ChangeState(TestStates.State2);
            builder.DefineState(TestStates.State2);

            grp.RegisterDependencies(Container);

            Assert.AreEqual(TestStates.Base, sm.CurrentState<TestStates>());
            var data = (sm.TypedStateGroup<TestStates>()
                //StateGroups[typeof(TestStates)] 
                ).CurrentStateData as State1Data;
            Assert.IsNull(data);

            await sm.GoToState(TestStates.State1);
            Assert.AreEqual(TestStates.State1, sm.CurrentState<TestStates>());
            data = (sm.TypedStateGroup<TestStates>()
                //StateGroups[typeof(TestStates)] 
                ).CurrentStateData as State1Data;
            Assert.IsNotNull(data);


            data.RaiseTestComplete(TestCompletion.Complete1);
            Assert.AreEqual(TestStates.State2, sm.CurrentState<TestStates>());
            var newdata = (sm.TypedStateGroup<TestStates>()
                //StateGroups[typeof(TestStates)] 
                ).CurrentStateData as State1Data;
            Assert.IsNull(newdata);
        }

        [TestMethod]
        public async Task TestOnCompleteChangeStateWithData()
        {
            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            var grp = sm.TypedStateGroup<TestStates>();//StateGroups[typeof(TestStates)] as EnumStateGroup<TestStates>;

            var sd = new State1Data();

            builder.DefineStateWithData<TestStates, State1Data>(TestStates.State1)
                .OnCompleteWithData(TestCompletion.Complete1, vm => vm.TestBoolValue)
                .ChangeState(TestStates.State2);
            builder.DefineStateWithData<TestStates, State2Data>(TestStates.State2)
                .WhenChangedToWithData((State2Data vm, int d, CancellationToken cancelToken) => vm.InitValue1 = $"Input: {d}");

            grp.RegisterDependencies(Container);

            Assert.AreEqual(TestStates.Base, sm.CurrentState<TestStates>());
            var data = (sm.TypedStateGroup<TestStates>()
                //StateGroups[typeof(TestStates)] 
                ).CurrentStateData as State1Data;
            Assert.IsNull(data);

            await sm.GoToState(TestStates.State1);
            Assert.AreEqual(TestStates.State1, sm.CurrentState<TestStates>());
            data = (sm.TypedStateGroup<TestStates>()
                //StateGroups[typeof(TestStates)] 
                ).CurrentStateData as State1Data;
            Assert.IsNotNull(data);


            data.RaiseTestComplete(TestCompletion.Complete1);
            Assert.AreEqual(TestStates.State2, sm.CurrentState<TestStates>());
            var newdata = (sm.TypedStateGroup<TestStates>()
                //StateGroups[typeof(TestStates)] 
                ).CurrentStateData as State2Data;
            Assert.IsNotNull(newdata);
            var input = newdata.InitValue1;
            Assert.IsNotNull(input);
        }

        [TestMethod]
        public async Task TestOnCompleteChangeStateWithDataEvent()
        {
            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            var grp = sm.TypedStateGroup<TestStates>();//StateGroups[typeof(TestStates)] as EnumStateGroup<TestStates>;

            builder
                .DefineStateWithData<TestStates, State1Data>(TestStates.State1)
                    .OnCompleteWithDataEvent<TestStates, State1Data, TestCompletion, int>(TestCompletion.Complete1)
                        .ChangeState(TestStates.State2)
                            .InitializeNewState<TestStates, State1Data, State2Data, int>(
                                (vm, d, cancelToken) => vm.InitValue1 = $"Custom init {d}")
                .DefineStateWithData<TestStates, State2Data>(TestStates.State2);
            grp.RegisterDependencies(Container);
            await ValidateOnComplete(sm);
        }

        private async Task ValidateOnComplete(StateManager sm)
        {
            Assert.AreEqual(TestStates.Base, sm.CurrentState<TestStates>());
            var data = (sm.TypedStateGroup<TestStates>()
            //StateGroups[typeof(TestStates)] 
            ).CurrentStateData as State1Data;
            Assert.IsNull(data);

            await sm.GoToState(TestStates.State1);
            Assert.AreEqual(TestStates.State1, sm.CurrentState<TestStates>());
            data = (sm.TypedStateGroup<TestStates>()
            //StateGroups[typeof(TestStates)] 
            ).CurrentStateData as State1Data;
            Assert.IsNotNull(data);


            data.RaiseTestCompleteWithData(TestCompletion.Complete1);
            Assert.AreEqual(TestStates.State2, sm.CurrentState<TestStates>());
            var newdata = (sm.TypedStateGroup<TestStates>()
            //StateGroups[typeof(TestStates)] 
            ).CurrentStateData as State2Data;
            Assert.IsNotNull(newdata);
            var input = newdata.InitValue1;
            Assert.IsNotNull(input);

        }

        [TestMethod]
        public async Task TestOnCompleteChangeStateInitialiseWithDataEvent()
        {
            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            var grp = sm.TypedStateGroup<TestStates>();

            builder
                .DefineStateWithData<TestStates, State1Data>(TestStates.State1)
                    .OnCompleteWithDataEvent<TestStates, State1Data, TestCompletion, int>(TestCompletion.Complete1)
                    .ChangeState(TestStates.State2)
                    .InitializeNewStateWithData<TestStates, State1Data, State2Data, int>()
                .DefineStateWithData<TestStates, State2Data>(TestStates.State2);
            grp.RegisterDependencies(Container);

            await ValidateOnComplete(sm);
        }

        [TestMethod]
        public async Task TestOnDefaultCompleteChangeState()
        {
            var sm = new StateManager();
            Assert.AreEqual(0, sm.StateGroups.Count);
            var builder = sm.Group<TestStates>();
            var grp = sm.TypedStateGroup<TestStates>();

            builder.DefineStateWithData<TestStates, State1Data>(TestStates.State1)
                .OnDefaultComplete()
                .ChangeState(TestStates.State2);
            builder.DefineState(TestStates.State2);

            grp.RegisterDependencies(Container);

            Assert.AreEqual(TestStates.Base, sm.CurrentState<TestStates>());
            var data = (sm.TypedStateGroup<TestStates>()
                //StateGroups[typeof(TestStates)] 
                ).CurrentStateData as State1Data;
            Assert.IsNull(data);

            await sm.GoToState(TestStates.State1);
            Assert.AreEqual(TestStates.State1, sm.CurrentState<TestStates>());
            data = (sm.TypedStateGroup<TestStates>()
                //StateGroups[typeof(TestStates)] 
                ).CurrentStateData as State1Data;
            Assert.IsNotNull(data);

            data.RaiseComplete();
            Assert.AreEqual(TestStates.State2, sm.CurrentState<TestStates>());
            var newdata = (sm.TypedStateGroup<TestStates>()
                //StateGroups[typeof(TestStates)] 
                ).CurrentStateData as State1Data;
            Assert.IsNull(newdata);
        }

        [TestMethod]
        public async Task TestStateLoggingFilter()
        {
            var count = 0;
            var assembly = typeof(StateHelpers).GetTypeInfo().Assembly.GetName().Name;
            LogHelper.LogService = new BasicLoggerService
            {
                LogOutput = entry =>
                {
                    count++;
                    Assert.AreEqual(assembly, entry.AssemblyName);
                    return Task.CompletedTask;
                },
                Filter = new AssemblyNameLogFilter
                {
                    AssemblyName = assembly
                }
            };

            var groupName = "test";
            var sg = new StateGroup(groupName);
            var stateName = "one";
            var sd = sg.TypedGroupDefinition.DefineStateFromName(stateName);
            await sg.ChangeToStateByName(stateName);
            await Task.Delay(100);
            Assert.IsTrue(count > 0);


            count = 0;
            LogHelper.LogService = new BasicLoggerService
            {
                LogOutput = entry =>
                {
                    count++;
                    Assert.AreNotEqual(assembly, entry.AssemblyName);
                    return Task.CompletedTask;
                },
                Filter = new NotLogFilter
                {
                    FilterToInvert = new AssemblyNameLogFilter
                    {
                        AssemblyName = assembly
                    }
                }
            };

            sg = new StateGroup(groupName);
            sd = sg.TypedGroupDefinition.DefineStateFromName(stateName);
            await sg.ChangeToStateByName(stateName);
            await Task.Delay(100);
            Assert.AreEqual(0,count );
        }
    }
}
