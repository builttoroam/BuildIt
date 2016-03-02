using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BuildIt.States.Completion;

namespace BuildIt.States
{
    public interface IStateBuilder
    {
        IStateManager StateManager { get; }
    }

    public interface IStateGroupBuilder<TState> : IStateBuilder
        where TState : struct
    {
        IStateGroup<TState> StateGroup { get; }
    }

    public interface IStateDefinitionBuilder<TState> : IStateGroupBuilder<TState>
    where TState : struct
    {
        IStateDefinition<TState> State { get; }
    }


    public interface IStateDefinitionWithDataBuilder<TState, TData> : IStateDefinitionBuilder<TState>
        where TData : INotifyPropertyChanged
        where TState : struct
    {
        IStateDefinitionTypedDataWrapper<TData> StateDataWrapper { get; }
    }

    public interface IStateDefinitionWithDataEventBuilder<TState, TData> : IStateDefinitionWithDataBuilder<TState, TData>
        where TData : INotifyPropertyChanged
        where TState : struct
    {
        Action<TData, EventHandler> Subscribe { get; }
        Action<TData, EventHandler> Unsubscribe { get; }

    }



    public interface IStateDefinitionValueTargetBuilder<TState, TElement> : IStateDefinitionBuilder<TState>
where TState : struct
    {
        TElement Target { get; }
    }


    public interface IStateDefinitionValueBuilder<TState,TElement, TPropertyValue> : IStateDefinitionBuilder<TState>
    where TState : struct
    {
        StateValue<TElement, TPropertyValue> Value { get; }
    }

    public interface IStateCompletionBuilder<TState, TCompletion> : IStateDefinitionBuilder<TState>
        where TState : struct
        where TCompletion : struct
    {
        TCompletion Completion { get;  }
    }

    public interface IStateCompletionWithDataBuilder<TState, TCompletion,TData> : IStateCompletionBuilder<TState,TCompletion>
        where TState : struct
        where TCompletion : struct
    {
        Func<TData> Data { get; }
    }

    public interface IStateWithDataCompletionBuilder<TState, TStateData, TCompletion> : 
         IStateDefinitionWithDataBuilder<TState, TStateData>, IStateCompletionBuilder<TState,TCompletion>
        where TState : struct
        where TCompletion : struct
        where TStateData: INotifyPropertyChanged
    {
    }

    public interface IStateWithDataCompletionWithDataBuilder<TState, TStateData, TCompletion, TData> : 
        IStateWithDataCompletionBuilder<TState, TStateData, TCompletion>
        where TState : struct
        where TCompletion : struct
        where TStateData : INotifyPropertyChanged
    {
        Func<TStateData,TData> Data { get; }
    }

    public static class StateHelpers
    {
        #region Builder Implementations
        private class StateBuilder : IStateBuilder
        {
            public IStateManager StateManager { get; set; }


        }

        private class StateGroupBuilder<TState> : StateBuilder, IStateGroupBuilder<TState>
            where TState : struct
        {
            public IStateGroup<TState> StateGroup { get; set; }

        }

        private class StateDefinitionBuilder<TState> : StateGroupBuilder<TState>, IStateDefinitionBuilder<TState>
            where TState : struct
        {
            public IStateDefinition<TState> State { get; set; }

        }

        private class StateDefinitionWithDataBuilder<TState,TData> : StateGroupBuilder<TState>, 
            IStateDefinitionWithDataBuilder<TState,TData>
            where TData:INotifyPropertyChanged
           where TState : struct
        {
            public IStateDefinition<TState> State { get; set; }
            public IStateDefinitionTypedDataWrapper<TData> StateDataWrapper
            => State.UntypedStateDataWrapper as IStateDefinitionTypedDataWrapper<TData>;
        }

        private class StateDefinitionWithDataEventBuilder<TState, TData> : StateDefinitionWithDataBuilder<TState, TData>,
            IStateDefinitionWithDataEventBuilder<TState, TData>
            where TData : INotifyPropertyChanged
           where TState : struct
        {
            public Action<TData, EventHandler> Subscribe { get; set; }
            public Action<TData, EventHandler> Unsubscribe { get; set; }

        }

        private class StateDefinitionValueTargetBuilder<TState, TElement> :
           StateDefinitionBuilder<TState>, IStateDefinitionValueTargetBuilder<TState, TElement>
          where TState : struct
        {
            public TElement Target  { get; set; }

        }

        private class StateDefinitionValueBuilder<TState,TElement, TPropertyValue> : 
            StateDefinitionBuilder<TState>, IStateDefinitionValueBuilder<TState,TElement, TPropertyValue>
           where TState : struct
        {
            public StateValue<TElement, TPropertyValue> Value { get; set; }

        }

        private class StateCompletionBuilder<TState, TCompletion> : StateDefinitionBuilder<TState>,
            IStateCompletionBuilder<TState, TCompletion>
            where TState:struct
            where TCompletion:struct
        {
            public TCompletion Completion { get; set; }
        }

        private class StateCompletionWithDataBuilder<TState, TCompletion,TData> : 
            StateCompletionBuilder<TState, TCompletion>,
    IStateCompletionWithDataBuilder<TState, TCompletion,TData>
    where TState : struct
    where TCompletion : struct
        {
            public Func<TData> Data{ get; set; }
        }

        private class StateWithDataCompletionBuilder<TState, TStateData, TCompletion>:
            StateCompletionBuilder<TState, TCompletion>,
            IStateWithDataCompletionBuilder<TState, TStateData, TCompletion> 
        where TState : struct
        where TCompletion : struct
        where TStateData : INotifyPropertyChanged
        {
            public IStateDefinitionTypedDataWrapper<TStateData> StateDataWrapper
                => State.UntypedStateDataWrapper as IStateDefinitionTypedDataWrapper<TStateData>;
        }

        private class StateWithDataCompletionWithDataBuilder<TState, TStateData, TCompletion, TData>:
            StateWithDataCompletionBuilder<TState, TStateData, TCompletion>,
            IStateWithDataCompletionWithDataBuilder<TState, TStateData, TCompletion, TData> 
            where TState : struct
            where TCompletion : struct
            where TStateData : INotifyPropertyChanged
        {
            public Func<TStateData,TData> Data { get; set; }
        }

        #endregion


        public static IStateGroupBuilder<TState> Group<TState>
            (this IStateManager vsm) where TState : struct

        {
            var grp = new StateGroup<TState>();
            vsm?.AddStateGroup(grp);
            return new StateGroupBuilder<TState>
            {
                StateManager = vsm,
                StateGroup = grp
            };
        }

        public static IStateGroupBuilder<TState> Group<TState>(
            this IStateBuilder vsmGroup)
            where TState : struct
        {
            return vsmGroup.StateManager.Group<TState>();
        }


       
        public static IStateGroupBuilder<TState> WithHistory<TState>(
    this IStateGroupBuilder<TState> vsmGroup)
    where TState : struct
        {
            vsmGroup.StateGroup.TrackHistory = true;
            return vsmGroup;
        }

        //public static IStateGroupBuilder<TState> 
        //    Group<TState, TExistingState>(
        //    this Tuple<IStateManager, IStateGroup<TExistingState>,
        //        StateDefinition<TExistingState>> vsmGroup, TState state)
        //    where TExistingState : struct
        //    where TState : struct
        //{
        //    var grp = new StateGroup<TState>();
        //    vsmGroup.Item1.AddStateGroup(grp);
        //    return new Tuple<IStateManager, IStateGroup<TState>>(vsmGroup.Item1, grp);
        //}


        //public static Tuple<IStateManager,
        //        IStateGroup<TState>> Group<TState, TExistingState, TExistingElement, TExistingPropertyValue>(
        //    this Tuple<IStateManager, IStateGroup<TExistingState>,
        //        StateDefinition<TExistingState>,
        //        StateValue<TExistingElement, TExistingPropertyValue>> vsmGroup, TState state)
        //    where TExistingState : struct
        //    where TState : struct
        //{
        //    var grp = new StateGroup<TState>();
        //    vsmGroup.Item1.AddStateGroup(grp);
        //    return new Tuple<IStateManager, IStateGroup<TState>>(vsmGroup.Item1, grp);
        //}

        public static IStateGroupBuilder<TState> DefineAllStates<TState>(
    this IStateGroupBuilder<TState> vsmGroup)
    where TState : struct
        {
            vsmGroup.StateGroup.DefineAllStates();
            return vsmGroup;
        }


        public static 
            IStateDefinitionBuilder<TState> DefineState<TState>(
            this IStateGroupBuilder<TState> vsmGroup,
            TState state)
            where TState : struct
        {
            var vs = new StateDefinition<TState> { State = state };
            return new StateDefinitionBuilder<TState>
            {
                StateManager = vsmGroup.StateManager,
                StateGroup = vsmGroup.StateGroup,
                State = vs
            };

            //vsmGroup.Item2.States.Add(state, vs);
            //return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinition<TState>>(vsmGroup.Item1, vsmGroup.Item2, vs);
        }

        //public static Tuple<IStateManager,
        //                    IStateGroup<TState>,
        //                    IStateDefinition<TState>> DefineState<TState>(
        //    this Tuple<IStateManager,
        //                IStateGroup<TState>,
        //                IStateDefinition<TState>> vsmGroup,
        //    TState state) where TState : struct
        //{
        //    var vs = new StateDefinition<TState> { State = state };
        //    vsmGroup.Item2.States.Add(state, vs);
        //    return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinition<TState>>(vsmGroup.Item1, vsmGroup.Item2, vs);
        //}

        public static IStateDefinitionBuilder<TState> AddTrigger<TState>(
            this IStateDefinitionBuilder<TState> vsmGroup,
            IStateTrigger trigger) where TState : struct
        {
            // Add trigger to triggers collection
            vsmGroup.State.Triggers.Add(trigger);

            // Advise the state group to monitor triggers
            vsmGroup.StateGroup.WatchTrigger(trigger);
            return vsmGroup;
        }

        //public static Tuple<IStateManager,
        //    IStateGroup<TState>,
        //    IStateDefinition<TState>> DefineState<TState, TExistingState, TExistingElement, TExistingPropertyValue>(
        //    this Tuple<IStateManager, IStateGroup<TState>, IStateDefinition<TExistingState>, StateValue<TExistingElement, TExistingPropertyValue>> vsmGroup, TState state)
        //    where TExistingState : struct
        //    where TState : struct
        //{
        //    var vs = new StateDefinition<TState> { State = state };
        //    vsmGroup.Item2.States.Add(state, vs);
        //    return new Tuple<IStateManager,
        //        IStateGroup<TState>, IStateDefinition<TState>>(vsmGroup.Item1, vsmGroup.Item2, vs);
        //}

        //public class StateCompletionBinder<TCompletion>
        //   where TCompletion : struct
        //{
        //    public TCompletion Completion { get; set; }

        //}

        public class StateEventBinder
        {
            public Action<EventHandler> Subscribe { get; set; }
            public Action<EventHandler> Unsubscribe { get; set; }
        }



        //public class StateCompletionWithDataBinder<TCompletion, TData> : StateCompletionBinder<TCompletion>
        //    where TCompletion : struct
        //{

        //    public Func<TData> CompletionData { get; set; }

        //}


        public static IStateCompletionBuilder<TState,DefaultCompletion>
          OnDefaultComplete<TState>(
          this IStateDefinitionBuilder<TState> smInfo)
          where TState : struct
        {
            return new StateCompletionBuilder<TState, DefaultCompletion>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = DefaultCompletion.Complete
            };

            //if (smInfo?.Item1 == null || smInfo.Item2 == null) return null;


            //var binder = new StateCompletionBinder<DefaultCompletion> { Completion = DefaultCompletion.Complete };
            //return smInfo.Extend(binder);
        }


        public static IStateCompletionBuilder<TState, TCompletion>
           OnComplete<TState, TCompletion>(
           this IStateDefinitionBuilder<TState> smInfo,
                TCompletion completion)
           where TState : struct
            where TCompletion : struct
        {
            return new StateCompletionBuilder<TState, TCompletion>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = completion
            };

            //if (smInfo?.Item1 == null || smInfo.Item2 == null) return null;


            //var binder = new StateCompletionBinder<TCompletion> { Completion = completion };
            //return smInfo.Extend(binder);
        }

        public static IStateWithDataCompletionBuilder<TState,TStateData, TCompletion>
            OnComplete<TState, TStateData,TCompletion>(
            this IStateDefinitionWithDataBuilder<TState,TStateData> smInfo,
            TCompletion completion)
            where TState : struct
            where TCompletion : struct
            where TStateData:INotifyPropertyChanged
        {
            return new StateWithDataCompletionBuilder<TState, TStateData,TCompletion>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = completion
            };
        }

        public static IStateCompletionWithDataBuilder<TState,DefaultCompletion,TData>
           OnDefaultCompleteWithData<TState, TData>(
           this IStateDefinitionBuilder<TState> smInfo,
                Func<TData> completionData)
           where TState : struct
        {
            return new StateCompletionWithDataBuilder<TState, DefaultCompletion,TData>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = DefaultCompletion.Complete,
                Data=completionData
            };

            //if (smInfo?.Item1 == null || smInfo.Item2 == null) return null;


            //var binder = new StateCompletionWithDataBinder<DefaultCompletion, TData>
            //{
            //    Completion = DefaultCompletion.Complete,
            //    CompletionData = completionData
            //};
            //return smInfo.Extend(binder);

        }

        public static
            IStateCompletionWithDataBuilder<TState, TCompletion, TData>
           OnCompleteWithData<TState, TCompletion, TData>(
           this
            IStateDefinitionBuilder<TState> smInfo,
                TCompletion completion,
                Func<TData> completionData)
           where TState : struct
            where TCompletion : struct
        {
            return new StateCompletionWithDataBuilder<TState, TCompletion, TData>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = completion,
                Data = completionData
            };
            //if (smInfo?.Item1 == null || smInfo.Item2 == null) return null;


            //var binder = new StateCompletionWithDataBinder<TCompletion, TData>
            //{
            //    Completion = completion,
            //    CompletionData = completionData
            //};
            //return smInfo.Extend(binder);

        }

        public static
            IStateDefinitionValueTargetBuilder<TState,TElement> Target<TState, TElement>(
            this IStateDefinitionBuilder<TState> vsmGroup, TElement element) where TState : struct
        {
            return new StateDefinitionValueTargetBuilder<TState, TElement>
            {
                StateManager = vsmGroup.StateManager,
                StateGroup = vsmGroup.StateGroup,
                State = vsmGroup.State,
                Target = element
            };
            //return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinition<TState>, TElement>(vsmGroup.Item1, vsmGroup.Item2, vsmGroup.Item3, element);
        }

        //public static Tuple<IStateManager,
        //    IStateGroup<TState>, IStateDefinition<TState>,
        //    TElement> Target<TState, TElement, TExistingElement, TExistingPropertyValue>(
        //    this Tuple<IStateManager, IStateGroup<TState>, IStateDefinition<TState>,
        //        StateValue<TExistingElement, TExistingPropertyValue>> vsmGroup, TElement element)
        //    where TState : struct

        //{
        //    return new Tuple<IStateManager,
        //        IStateGroup<TState>, IStateDefinition<TState>,
        //        TElement>(vsmGroup.Item1, vsmGroup.Item2, vsmGroup.Item3, element);
        //}


        public static
            IStateDefinitionValueBuilder<TState, TElement,TPropertyValue>
            Change<TState, TElement, TPropertyValue>(
            this IStateDefinitionValueTargetBuilder<TState, TElement> vsmGroup,
            Expression<Func<TElement, TPropertyValue>> getter,
            Action<TElement, TPropertyValue> setter) where TState : struct
        {
            var vsv = new StateValue<TElement, TPropertyValue>
            {
                Key = new Tuple<object, string>(vsmGroup.Target, (getter.Body as MemberExpression)?.Member.Name),
                Element = vsmGroup.Target,
                Getter = getter.Compile(),
                Setter = setter
            };
            return new StateDefinitionValueBuilder<TState, TElement,TPropertyValue>
            {
                StateManager = vsmGroup.StateManager,
                StateGroup = vsmGroup.StateGroup,
                State = vsmGroup.State,
                Value=vsv
            };
            //return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinition<TState>, StateValue<TElement, TPropertyValue>>(
            //    vsmGroup.Item1, vsmGroup.Item2, vsmGroup.Item3, vsv);
        }

        public static
            IStateDefinitionValueBuilder<TState, TElement, TPropertyValue>
            ToValue<TState, TElement, TPropertyValue>(
            this
            IStateDefinitionValueBuilder<TState, TElement, TPropertyValue> vsmGroup,
            TPropertyValue value) where TState : struct
        {
            vsmGroup.Value.Value = value;
            vsmGroup.State.Values.Add(vsmGroup.Value);
            //vsmGroup.Item4.Value = value;
            //vsmGroup.Item3.Values.Add(vsmGroup.Item4);
            return vsmGroup;
        }

        public static StateValue<TElement, TPropertyValue> ChangeProperty<TElement, TPropertyValue>(
            this TElement element, Expression<Func<TElement, TPropertyValue>> getter, Action<TElement, TPropertyValue> setter)

        {
            return new StateValue<TElement, TPropertyValue>
            {
                Key = new Tuple<object, string>(element, (getter.Body as MemberExpression)?.Member.Name),
                Element = element,
                Getter = getter.Compile(),
                Setter = setter
            };
        }

        public static StateValue<TElement, TPropertyValue> ToValue<TElement, TPropertyValue>(
            this StateValue<TElement, TPropertyValue> state, TPropertyValue newValue)

        {
            state.Value = newValue;
            return state;
        }

        public static
            IStateDefinitionBuilder<TState> WhenAboutToChange<TState>(
           this IStateDefinitionBuilder<TState> stateDefinition,
           Action<CancelEventArgs> action) where TState : struct
        {
#pragma warning disable 1998  // Convert sync method into async call
            return stateDefinition.WhenAboutToChange(async cancel => action(cancel));
#pragma warning restore 1998
        }

        public static IStateDefinitionBuilder<TState> WhenAboutToChange<TState>(
            this IStateDefinitionBuilder<TState>  smInfo,
            Func<CancelEventArgs, Task> action) where TState : struct
        {
            if (smInfo?.State == null) return null;
            var stateDefinition = smInfo.State;

            "Adding Behaviour: AboutToChangeFrom".Log();
            stateDefinition.AboutToChangeFrom = action;
            return smInfo;
        }

        public static IStateDefinitionBuilder<TState> WhenChangingFrom<TState>(
            this IStateDefinitionBuilder<TState> stateDefinition,
            Action action) where TState : struct
        {
#pragma warning disable 1998  // Convert sync method into async call
            return stateDefinition.WhenChangingFrom(async () => action());
#pragma warning restore 1998
        }

        public static IStateDefinitionBuilder<TState> WhenChangingFrom<TState>(
            this IStateDefinitionBuilder<TState> smInfo,
            Func<Task> action) where TState : struct
        {
            if (smInfo?.State == null) return null;
            var stateDefinition = smInfo.State;

            "Adding Behaviour: ChangingFrom".Log();
            stateDefinition.ChangingFrom = action;
            return smInfo;
        }

        public static IStateDefinitionBuilder<TState> WhenChangedTo<TState>(
            this IStateDefinitionBuilder<TState> smInfo,
            Action action) where TState : struct
        {
#pragma warning disable 1998  // Convert sync method into async call
            return smInfo.WhenChangedTo(async () => action());
#pragma warning restore 1998
        }

        public static IStateDefinitionBuilder<TState> WhenChangedTo<TState>(
            this IStateDefinitionBuilder<TState> smInfo,
            Func<Task> action) where TState : struct
        {
            if (smInfo?.State == null) return null;
            var stateDefinition = smInfo.State;

            "Adding Behaviour: ChangedTo".Log();
            stateDefinition.ChangedTo = action;
            return smInfo;
        }


        //public static ITransitionDefinition<TState> From<TState>(this ITransitionDefinition<TState> transition,
        //    TState state) where TState : struct
        //{
        //    $"Defining start state {state}".Log();
        //    transition.StartState = state;
        //    return transition;
        //}

        //public static ITransitionDefinition<TState> To<TState>(this ITransitionDefinition<TState> transition,
        //    TState state) where TState : struct
        //{
        //    $"Defining end state {state}".Log();
        //    transition.EndState = state;
        //    return transition;
        //}

        #region Create State
        public static
            IStateDefinitionWithDataBuilder<TState,TStateData>
           StateWithStateData<TState, TStateData>(
                this IStateGroupBuilder<TState> smInfo,
                TState state)
            where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            var vms = smInfo.StateGroup.DefineStateWithData<TStateData>(state);
            return new StateDefinitionWithDataBuilder<TState, TStateData>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = vms.State
            };
        }

        //public static Tuple<IStateManager, IStateGroup<TState>>
        //    EndState<TState, TStateData>(this Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>> smInfo)
        //    where TState : struct
        //    where TStateData : INotifyPropertyChanged
        //{
        //    return smInfo.Remove();
        //}

        #endregion

        //public static Tuple<IStateManager,
        //    IStateGroup<TState>,
        //    IStateDefinition<TState>> AsState<TState, TStateData>(
        //    this Tuple<IStateManager,
        //        IStateGroup<TState>,
        //        IStateDefinitionWithData<TState, TStateData>> smInfo)
        //    where TState : struct
        //    where TStateData : INotifyPropertyChanged
        //{
        //    return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinition<TState>>(
        //        smInfo.Item1, smInfo.Item2, smInfo.Item3.State
        //        );
        //}

        //public static Tuple<IStateManager,
        //        IStateGroup<TState>,
        //        IStateDefinitionWithData<TState, TStateData>> AsStateWithStateData<TState, TStateData>(
        //    this Tuple<IStateManager,
        //    IStateGroup<TState>,
        //    IStateDefinition<TState>> smInfo)
        //    where TState : struct
        //    where TStateData : INotifyPropertyChanged
        //{
        //    return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>>(
        //        smInfo.Item1, smInfo.Item2, new StateDefinitionWithDataWrapper<TState, TStateData> { State = smInfo.Item3 }
        //        );
        //}

        public static

            IStateDefinitionWithDataBuilder<TState,TStateData>
            Initialise<TState, TStateData>(
                this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
                Action<TStateData> action)
                where TState : struct
                where TStateData : INotifyPropertyChanged

        {
#pragma warning disable 1998 // Convert sync method into async call
            return smInfo.Initialise(async vm => action(vm));
#pragma warning restore 1998
        }

        public static IStateDefinitionWithDataBuilder<TState, TStateData> Initialise<TState, TStateData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
            Func<TStateData, Task> action)
            where TState : struct
            where TStateData : INotifyPropertyChanged

        {
            if (smInfo?.StateDataWrapper == null) return null;
            var stateDefinition = smInfo.StateDataWrapper;


            "Adding Initialization".Log();
            if (stateDefinition.Initialise == null || action == null)
            {
                stateDefinition.Initialise = action;
            }
            else
            {
                stateDefinition.Initialise += action;
            }
            return smInfo;
        }

        public static IStateDefinitionWithDataBuilder<TState, TStateData>  WhenAboutToChange<TState, TStateData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> stateDefinition,
            Action<TStateData, CancelEventArgs> action) where TState : struct
            where TStateData : INotifyPropertyChanged
        {
#pragma warning disable 1998 // Convert sync method into async call
            return stateDefinition.WhenAboutToChange(async (vm, cancel) => action(vm, cancel));
#pragma warning restore 1998
        }

        public static IStateDefinitionWithDataBuilder<TState, TStateData> WhenAboutToChange<TState, TStateData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
            Func<TStateData, CancelEventArgs, Task> action) where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            if (smInfo?.StateDataWrapper == null) return null;
            var stateDefinition = smInfo.StateDataWrapper;

            "Adding Behaviour: AboutToChangeFromViewModel".Log();
            if (stateDefinition.AboutToChangeFrom == null || action == null)
            {
                stateDefinition.AboutToChangeFrom = action;
            }
            else
            {
                stateDefinition.AboutToChangeFrom += action;
            }
            return smInfo;
        }

        public static IStateDefinitionWithDataBuilder<TState, TStateData> WhenChangingFrom<TState, TStateData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
            Action<TStateData> action) where TState : struct
            where TStateData : INotifyPropertyChanged
        {
#pragma warning disable 1998  // Convert sync method into async call
            return smInfo.WhenChangingFrom(async vm => action(vm));
#pragma warning restore 1998
        }

        public static IStateDefinitionWithDataBuilder<TState, TStateData> WhenChangingFrom<TState, TStateData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
            Func<TStateData, Task> action) where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            if (smInfo?.StateDataWrapper == null) return null;
            var stateDefinition = smInfo.StateDataWrapper;

            "Adding Behaviour: ChangingFromViewModel".Log();
            if (stateDefinition.ChangingFrom == null || action == null)
            {
                stateDefinition.ChangingFrom = action;
            }
            else
            {
                stateDefinition.ChangingFrom += action;
            }
            return smInfo;
        }

        public static IStateDefinitionWithDataBuilder<TState, TStateData> WhenChangedTo<TState, TStateData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData>  smInfo,
            Action<TStateData> action) where TState : struct
            where TStateData : INotifyPropertyChanged
        {
#pragma warning disable 1998  // Convert sync method into async call
            return smInfo.WhenChangedTo(async vm => action(vm));
#pragma warning restore 1998
        }

        public static IStateDefinitionWithDataBuilder<TState, TStateData> WhenChangedTo<TState, TStateData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
            Func<TStateData, Task> action) where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            if (smInfo?.StateDataWrapper == null) return null;
            var stateDefinition = smInfo.StateDataWrapper;

            "Adding Behaviour: ChangedToViewModel".Log();

            if (stateDefinition.ChangedTo == null || action == null)
            {
                stateDefinition.ChangedTo = action;
            }
            else
            {
                stateDefinition.ChangedTo += action;
            }
            return smInfo;
        }

        public static IStateDefinitionWithDataBuilder<TState, TStateData> WhenChangedToWithData<TState, TStateData, TData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
            Action<TStateData, TData> action) where TState : struct
            where TStateData : INotifyPropertyChanged
        {
#pragma warning disable 1998  // Convert sync method into async call
            return smInfo.WhenChangedToWithData<TState, TStateData, TData>(async (vm, s) => action(vm, s));
#pragma warning restore 1998
        }

        public static IStateDefinitionWithDataBuilder<TState, TStateData> WhenChangedToWithData<TState, TStateData, TData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
            Func<TStateData, TData, Task> action) where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            if (smInfo?.StateDataWrapper == null) return null;
            var stateDefinition = smInfo.StateDataWrapper;

            "Adding Behaviour: ChangedToWithDataViewModel".Log();

            var modAction = new Func<TStateData, string, Task>((vm, d) =>
            {
                var data = d.DecodeJson<TData>();
                return action(vm, data);
            });

            if (action == null)
            {
                stateDefinition.ChangedToWithData = null;
            }
            else
            {
                stateDefinition.ChangedToWithData += modAction;
            }
            return smInfo;
        }

        public static IStateDefinitionWithDataEventBuilder<TState,TStateData>
            
            OnEvent<TState, TStateData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
            Action<TStateData, EventHandler> subscribe,
            Action<TStateData, EventHandler> unsubscribe)
            where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            if (smInfo?.State == null) return null;
            return new StateDefinitionWithDataEventBuilder<TState, TStateData>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Subscribe = subscribe,
                Unsubscribe = unsubscribe
            };

            //if (smInfo?.Item1 == null || smInfo.Item2 == null) return null;

            //var binder = new ViewModelStateEventBinder<TStateData> { Subscribe = subscribe, Unsubscribe = unsubscribe };
            //return smInfo.Extend(binder);

            //new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>, StateEventBinder<TStateData>>(
            //smInfo.Item1, smInfo.Item2, smInfo.Item3, binder);
            //"Adding Behaviour: ChangedToViewModel".Log();
            //stateDefinition.ChangedToViewModel = action;
            //return stateDefinition;
        }


        public static 
            IStateCompletionWithDataBuilder<TState,DefaultCompletion,TStateData>
           OnDefaultComplete<TState, TStateData>(
           this 
            IStateDefinitionWithDataBuilder<TState,TStateData> smInfo)
           where TState : struct
           where TStateData : INotifyPropertyChanged, ICompletion<DefaultCompletion>
        {
            if (smInfo?.State == null) return null;

            return new StateCompletionWithDataBuilder<TState, DefaultCompletion, TStateData>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = DefaultCompletion.Complete
            };

            //var binder = new StateCompletionBinder<DefaultCompletion> { Completion = DefaultCompletion.Complete };
            //return smInfo.Extend(binder);
        }


        //public static Tuple<IStateManager,
        //    IStateGroup<TState>,
        //    IStateDefinitionWithData<TState, TStateData>,
        //    StateCompletionBinder<TCompletion>>
        //   OnComplete<TState, TStateData, TCompletion>(
        //   this Tuple<IStateManager,
        //       IStateGroup<TState>,
        //       IStateDefinitionWithData<TState, TStateData>> smInfo,
        //        TCompletion completion)
        //   where TState : struct
        //   where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
        //    where TCompletion : struct
        //{
        //    if (smInfo?.Item1 == null || smInfo.Item2 == null) return null;


        //    var binder = new StateCompletionBinder<TCompletion> { Completion = completion };
        //    return smInfo.Extend(binder);
        //}

        //public static Tuple<IStateManager,
        //    IStateGroup<TState>,
        //    IStateDefinitionWithData<TState, TStateData>,
        //    ViewModelStateCompletionWithDataBinder<TStateData, DefaultCompletion, TData>>
        //   OnDefaultCompleteWithData<TState, TStateData, TData>(
        //   this Tuple<IStateManager,
        //       IStateGroup<TState>,
        //       IStateDefinitionWithData<TState, TStateData>> smInfo,
        //        Func<TStateData, TData> completionData)
        //   where TState : struct
        //   where TStateData : INotifyPropertyChanged, ICompletion<DefaultCompletion>
        //{
        //    if (smInfo?.Item1 == null || smInfo.Item2 == null) return null;


        //    var binder = new ViewModelStateCompletionWithDataBinder<TStateData, DefaultCompletion, TData> { Completion = DefaultCompletion.Complete, CompletionData = completionData };
        //    return smInfo.Extend(binder);

        //}

        public static IStateWithDataCompletionWithDataBuilder<TState, TStateData,TCompletion, TData>
           OnCompleteWithData<TState, TStateData, TCompletion, TData>(
           this
            IStateDefinitionWithDataBuilder<TState,TStateData> smInfo,
                TCompletion completion,
                Func<TStateData, TData> completionData)
           where TState : struct
           where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
            where TCompletion : struct
        {
            return new StateWithDataCompletionWithDataBuilder<TState, TStateData,TCompletion, TData>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = completion,
                Data = completionData
            };

            //var binder = new ViewModelStateCompletionWithDataBinder<TStateData, TCompletion, TData> { Completion = completion, CompletionData = completionData };
            //return smInfo.Extend(binder);

        }

        public static IStateDefinitionWithDataBuilder<TState, TStateData> ChangeState<TState, TStateData>(
            this IStateDefinitionWithDataEventBuilder<TState,TStateData> smInfo,
            TState stateToChangeTo) where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            if (smInfo?.State == null) return null;
            //if (smInfo?.Item1 == null ||
            //    smInfo.Item2 == null ||
            //    smInfo.Item3 == null) return null;

            var binder = smInfo;
            var sm = smInfo.StateManager;
            var newState = stateToChangeTo;
            var changeStateAction = new EventHandler((s, e) =>
            {
                sm.GoToState(newState);
            });

            var smreturn = smInfo;

            var returnd = smreturn.WhenChangedTo(vm =>
            {
                binder.Subscribe(vm, changeStateAction);
            })
                .WhenChangingFrom(vm =>
                {
                    binder.Unsubscribe(vm, changeStateAction);
                });

            return returnd;

            //new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }

        public static IStateDefinitionWithDataBuilder<TState, TStateData>
            ChangeToPreviousState<TState, TStateData>(
            this IStateDefinitionWithDataEventBuilder<TState, TStateData> smInfo) 
            where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            if (smInfo.State == null) return null;
            //if (smInfo?.Item1 == null ||
            //    smInfo.Item2 == null ||
            //    smInfo.Item3 == null) return null;

            var binder = smInfo;
            var sm = smInfo.StateManager;
            var changeStateAction = new EventHandler((s, e) =>
            {
                sm.GoBackToPreviousState();
            });

            var smreturn = smInfo;

            var returnd = smreturn.WhenChangedTo(vm =>
            {
                binder.Subscribe(vm, changeStateAction);
            })
                .WhenChangingFrom(vm =>
                {
                    binder.Unsubscribe(vm, changeStateAction);
                });
            return returnd;
            //            return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }

        public static 
            IStateDefinitionWithDataBuilder<TState,TStateData> 
            ChangeState<TState, TStateData, TCompletion>(
            this IStateWithDataCompletionBuilder<TState, TStateData,TCompletion> smInfo,
            TState stateToChangeTo)
            where TState : struct
            where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
            where TCompletion : struct
        {
            if (smInfo?.State == null) return null;
            //if (smInfo?.Item1 == null ||
            //    smInfo.Item2 == null ||
            //    smInfo.Item3 == null) return null;

            var comp = smInfo?.Completion;
            var sm = smInfo?.StateManager;
            var newState = stateToChangeTo;
            var changeStateAction = new EventHandler<CompletionEventArgs<TCompletion>>((s, e) =>
            {
                if (e.Completion.Equals(comp))
                {
                    sm.GoToState(newState);
                }
            });

            var smreturn = smInfo;

            var returnd = smreturn.WhenChangedTo(vm =>
            {
                vm.Complete += changeStateAction;
            })
                .WhenChangingFrom(vm =>
                {
                    vm.Complete -= changeStateAction;
                });
            return smreturn;
            //            return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }


        public static 
            IStateDefinitionWithDataBuilder<TState,TStateData>
            ChangeState<TState, TStateData, TCompletion, TData>(
            this  IStateWithDataCompletionWithDataBuilder<TState,TStateData,TCompletion,TData> smInfo,
            TState stateToChangeTo)
            where TState : struct
            where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
            where TCompletion : struct
        {
            //if (smInfo?.Item1 == null ||
            //    smInfo.Item2 == null ||
            //    smInfo.Item3 == null) return null;

            var comp = smInfo?.Completion;
            var data = smInfo?.Data;
            var sm = smInfo.StateManager;
            var newState = stateToChangeTo;
            var changeStateAction = new EventHandler<CompletionEventArgs<TCompletion>>((s, e) =>
            {
                var dataVal = default(TData);
                var cc = e as CompletionWithDataEventArgs<TCompletion, TData>;
                if (cc != null)
                {
                    dataVal = cc.Data;
                }
                if (data != null)
                {
                    var vvm = (TStateData)s;
                    dataVal = data(vvm);
                }
                if (e.Completion.Equals(comp))
                {
                    sm.GoToStateWithData(newState, dataVal);
                }
            });



            var smreturn = smInfo;

            smreturn.WhenChangedTo(vm =>
            {
                vm.Complete += changeStateAction;
            })
                .WhenChangingFrom(vm =>
                {
                    vm.Complete -= changeStateAction;
                });
            return smreturn;
            //            return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }
        public static
            IStateDefinitionWithDataBuilder<TState, TStateData>
            ChangeToPreviousState<TState, TStateData, TCompletion>(
            this
            IStateWithDataCompletionBuilder<TState,TStateData,TCompletion> smInfo
            )
            where TState : struct
            where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
            where TCompletion : struct
        {
            //if (smInfo?.Item1 == null ||
            //    smInfo.Item2 == null ||
            //    smInfo.Item3 == null) return null;

            var comp = smInfo?.Completion;
            var sm = smInfo.StateManager;
            var changeStateAction = new EventHandler<CompletionEventArgs<TCompletion>>((s, e) =>
            {
                if (e.Completion.Equals(comp))
                {
                    sm.GoBackToPreviousState();
                }
            });



            var smreturn = smInfo;

            var returnd = smreturn.WhenChangedTo(vm =>
            {
                vm.Complete += changeStateAction;
            })
                .WhenChangingFrom(vm =>
                {
                    vm.Complete -= changeStateAction;
                });
            return returnd;
            //            return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }

        

        //public class ViewModelStateEventBinder<TStateData> where TStateData : INotifyPropertyChanged
        //{
        //    public Action<TStateData, EventHandler> Subscribe { get; set; }
        //    public Action<TStateData, EventHandler> Unsubscribe { get; set; }
        //}



        //public class ViewModelStateCompletionWithDataBinder<TStateData, TCompletion, TData>
        //    : StateCompletionBinder<TCompletion>
        //    where TCompletion : struct
        //    where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
        //{

        //    public Func<TStateData, TData> CompletionData { get; set; }

        //}


    }

    public class TypeRef
    {
        // ReSharper disable once UnusedTypeParameter - This is used to generate a known generic from 
        // a type reference
        public class TypeWrapper<TType>
        { }

        public static TypeWrapper<TType> Get<TType>()
        {
            return new TypeWrapper<TType>();
        }

    }


}