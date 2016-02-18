using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BuildIt.States
{
    public static class StateHelpers
    {
        public static Tuple<IStateManager, StateGroup<TState>> Group<TState>(
            this IStateManager vsm) where TState : struct
        {
            return vsm.GroupWithTransition<TState>();
            //var grp = new StateGroup<TState>();
            //vsm.StateGroups.Add(typeof(TState), grp);
            //return new Tuple<IStateManager, StateGroup<TState>>(vsm, grp);
        }

        public static Tuple<IStateManager, StateGroup<TState>> Group<TState>(
            this Tuple<IStateManager, StateGroup<TState>> vsmGroup)
            where TState : struct
        {
            return vsmGroup.Item1.Group<TState>();
        }

        public static Tuple<IStateManager, StateGroup<TState>> GroupWithTransition<TState>(
            this IStateManager vsm) where TState : struct
            
        {
            var grp = new StateGroup<TState>();
            vsm.AddStateGroup(grp);
            return new Tuple<IStateManager, StateGroup<TState>>(vsm, grp);
        }

        public static Tuple<IStateManager, StateGroup<TState>> GroupWithTransition<TState>(
            this Tuple<IStateManager, StateGroup<TState>> vsmGroup)
            where TState : struct
            
        {
            return vsmGroup.Item1.GroupWithTransition<TState>();
        }

        public static Tuple<IStateManager,
            StateGroup<TState>> Group<TState, TExistingState>(
            this Tuple<IStateManager, StateGroup<TExistingState>,
                StateDefinition<TExistingState>> vsmGroup, TState state)
            where TExistingState : struct
            where TState : struct
        {
            var grp = new StateGroup<TState>();
            vsmGroup.Item1.AddStateGroup(grp);
            return new Tuple<IStateManager, StateGroup<TState>>(vsmGroup.Item1, grp);
        }


        public static Tuple<IStateManager,
            StateGroup<TState>> Group<TState, TExistingState, TExistingElement, TExistingPropertyValue>(
            this Tuple<IStateManager, StateGroup<TExistingState>,
                StateDefinition<TExistingState>,
                StateValue<TExistingElement, TExistingPropertyValue>> vsmGroup, TState state)
            where TExistingState : struct
            where TState : struct
        {
            var grp = new StateGroup<TState>();
            vsmGroup.Item1.AddStateGroup(grp);
            return new Tuple<IStateManager, StateGroup<TState>>(vsmGroup.Item1, grp);
        }

        public static Tuple<IStateManager, StateGroup<TState>> DefineAllStates<TState>(
    this Tuple<IStateManager, StateGroup<TState>> vsmGroup)
    where TState : struct
        {
            vsmGroup.Item2.DefineAllStates();
            return vsmGroup;
        }


        public static Tuple<IStateManager, 
                            StateGroup<TState>,
                            StateDefinition<TState>> DefineState<TState>(
            this Tuple<IStateManager, 
                        StateGroup<TState>> vsmGroup, 
            TState state)
            where TState : struct
        {
            var vs = new StateDefinition<TState> { State = state };
            vsmGroup.Item2.States.Add(state, vs);
            return new Tuple<IStateManager, StateGroup<TState>, StateDefinition<TState>>(vsmGroup.Item1, vsmGroup.Item2, vs);
        }

        public static Tuple<IStateManager, 
                            StateGroup<TState>, 
                            StateDefinition<TState>> DefineState<TState>(
            this Tuple<IStateManager, 
                        StateGroup<TState>, 
                        StateDefinition<TState>> vsmGroup, 
            TState state) where TState : struct
        {
            var vs = new StateDefinition<TState> { State = state };
            vsmGroup.Item2.States.Add(state, vs);
            return new Tuple<IStateManager, StateGroup<TState>, StateDefinition<TState>>(vsmGroup.Item1, vsmGroup.Item2, vs);
        }

        public static Tuple<IStateManager,
                            StateGroup<TState>,
                            StateDefinition<TState>> AddTrigger<TState>(
            this Tuple<IStateManager,
                        StateGroup<TState>,
                        StateDefinition<TState>> vsmGroup,
            IStateTrigger trigger) where TState : struct
        {
            // Add trigger to triggers collection
            vsmGroup.Item3.Triggers.Add(trigger);

            // Advise the state group to monitor triggers
            vsmGroup.Item2.WatchTrigger(trigger);
            return vsmGroup;
        }

        public static Tuple<IStateManager,
            StateGroup<TState>, StateDefinition<TState>> DefineState<TState, TExistingState, TExistingElement, TExistingPropertyValue>(
            this Tuple<IStateManager, StateGroup<TState>, StateDefinition<TExistingState>, StateValue<TExistingElement, TExistingPropertyValue>> vsmGroup, TState state)
            where TExistingState : struct
            where TState : struct
        {
            var vs = new StateDefinition<TState> { State = state };
            vsmGroup.Item2.States.Add(state, vs);
            return new Tuple<IStateManager,
                StateGroup<TState>, StateDefinition<TState>>(vsmGroup.Item1, vsmGroup.Item2, vs);
        }

        public class StateCompletionBinder<TCompletion>
           where TCompletion : struct
        {
            public TCompletion Completion { get; set; }

        }

        public class StateEventBinder 
        {
            public Action<EventHandler> Subscribe { get; set; }
            public Action<EventHandler> Unsubscribe { get; set; }
        }



        public class StateCompletionWithDataBinder<TCompletion, TData> : StateCompletionBinder<TCompletion>
            where TCompletion : struct
        {

            public Func<TData> CompletionData { get; set; }

        }


        public static Tuple<IStateManager,
           IStateGroup<TState>,
           IStateDefinition<TState>,
           StateCompletionBinder<DefaultCompletion>>
          OnDefaultComplete<TState>(
          this Tuple<IStateManager,
              IStateGroup<TState>,
              IStateDefinition<TState>> smInfo)
          where TState : struct
        {
            if (smInfo?.Item1 == null || smInfo.Item2 == null) return null;


            var binder = new StateCompletionBinder<DefaultCompletion> { Completion = DefaultCompletion.Complete };
            return smInfo.Extend(binder);
        }


        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinition<TState>,
            StateCompletionBinder<TCompletion>>
           OnComplete<TState, TViewModel, TCompletion>(
           this Tuple<IStateManager,
               IStateGroup<TState>,
               IStateDefinition<TState>> smInfo,
                TCompletion completion)
           where TState : struct
            where TCompletion : struct
        {
            if (smInfo?.Item1 == null || smInfo.Item2 == null) return null;


            var binder = new StateCompletionBinder<TCompletion> { Completion = completion };
            return smInfo.Extend(binder);
        }

        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinition<TState>,
            StateCompletionWithDataBinder<DefaultCompletion, TData>>
           OnDefaultCompleteWithData<TState, TData>(
           this Tuple<IStateManager,
               IStateGroup<TState>,
               IStateDefinition<TState>> smInfo,
                Func<TData> completionData)
           where TState : struct
        {
            if (smInfo?.Item1 == null || smInfo.Item2 == null) return null;


            var binder = new StateCompletionWithDataBinder<DefaultCompletion, TData>
            {
                Completion = DefaultCompletion.Complete, CompletionData = completionData
            };
            return smInfo.Extend(binder);

        }

        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinition<TState>,
            StateCompletionWithDataBinder<TCompletion, TData>>
           OnCompleteWithData<TState, TCompletion, TData>(
           this Tuple<IStateManager,
               IStateGroup<TState>,
               IStateDefinition<TState>> smInfo,
                TCompletion completion,
                Func<TData> completionData)
           where TState : struct
            where TCompletion : struct
        {
            if (smInfo?.Item1 == null || smInfo.Item2 == null) return null;


            var binder = new StateCompletionWithDataBinder<TCompletion, TData>
            {
                Completion = completion, CompletionData = completionData
            };
            return smInfo.Extend(binder);

        }

        public static Tuple<IStateManager,
            StateGroup<TState>, StateDefinition<TState>,
            TElement> Target<TState, TElement>(
            this Tuple<IStateManager, StateGroup<TState>, StateDefinition<TState>> vsmGroup, TElement element) where TState : struct
        {
            return new Tuple<IStateManager, StateGroup<TState>, StateDefinition<TState>, TElement>(vsmGroup.Item1, vsmGroup.Item2, vsmGroup.Item3, element);
        }

        public static Tuple<IStateManager,
            StateGroup<TState>, StateDefinition<TState>,
            TElement> Target<TState, TElement, TExistingElement, TExistingPropertyValue>(
            this Tuple<IStateManager, StateGroup<TState>, StateDefinition<TState>,
                StateValue<TExistingElement, TExistingPropertyValue>> vsmGroup, TElement element)
            where TState : struct

        {
            return new Tuple<IStateManager,
                StateGroup<TState>, StateDefinition<TState>,
                TElement>(vsmGroup.Item1, vsmGroup.Item2, vsmGroup.Item3, element);
        }


        public static Tuple<IStateManager,
            StateGroup<TState>, StateDefinition<TState>, StateValue<TElement, TPropertyValue>> Change<TState, TElement, TPropertyValue>(
            this Tuple<IStateManager,
                StateGroup<TState>, StateDefinition<TState>,
                TElement> vsmGroup,
            Expression<Func<TElement, TPropertyValue>> getter,
            Action<TElement, TPropertyValue> setter) where TState : struct
        {
            var vsv = new StateValue<TElement, TPropertyValue>
            {
                Key = new Tuple<object, string>(vsmGroup.Item4, (getter.Body as MemberExpression)?.Member.Name),
                Element = vsmGroup.Item4,
                Getter = getter.Compile(),
                Setter = setter
            };
            return new Tuple<IStateManager, StateGroup<TState>, StateDefinition<TState>, StateValue<TElement, TPropertyValue>>(
                vsmGroup.Item1, vsmGroup.Item2, vsmGroup.Item3, vsv);
        }

        public static Tuple<IStateManager, StateGroup<TState>, StateDefinition<TState>,
            StateValue<TElement, TPropertyValue>> ToValue<TState, TElement, TPropertyValue>(
            this Tuple<IStateManager, StateGroup<TState>, StateDefinition<TState>,
                StateValue<TElement, TPropertyValue>> vsmGroup,
            TPropertyValue value) where TState : struct
        {
            vsmGroup.Item4.Value = value;
            vsmGroup.Item3.Values.Add(vsmGroup.Item4);
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

        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinition<TState>> WhenAboutToChange<TState>(
           this Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinition<TState>> stateDefinition,
           Action<CancelEventArgs> action) where TState : struct
        {
#pragma warning disable 1998  // Convert sync method into async call
            return stateDefinition.WhenAboutToChange(async cancel => action(cancel));
#pragma warning restore 1998
        }

        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinition<TState>> WhenAboutToChange<TState>(
            this Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinition<TState>> smInfo,
            Func<CancelEventArgs, Task> action) where TState : struct
        {
            if (smInfo?.Item3 == null) return null;
            var stateDefinition = smInfo.Item3;

            "Adding Behaviour: AboutToChangeFrom".Log();
            stateDefinition.AboutToChangeFrom = action;
            return smInfo;
        }

        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinition<TState>> WhenChangingFrom<TState>(
            this Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinition<TState>> stateDefinition,
            Action action) where TState : struct
        {
#pragma warning disable 1998  // Convert sync method into async call
            return stateDefinition.WhenChangingFrom(async () => action());
#pragma warning restore 1998
        }

        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinition<TState>> WhenChangingFrom<TState>(
            this Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinition<TState>> smInfo,
            Func<Task> action) where TState : struct
        {
            if (smInfo?.Item3 == null) return null;
            var stateDefinition = smInfo.Item3;

            "Adding Behaviour: ChangingFrom".Log();
            stateDefinition.ChangingFrom = action;
            return smInfo;
        }

        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinition<TState>> WhenChangedTo<TState>(
            this Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinition<TState>> smInfo,
            Action action) where TState : struct
        {
#pragma warning disable 1998  // Convert sync method into async call
            return smInfo.WhenChangedTo(async () => action());
#pragma warning restore 1998
        }

        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinition<TState>>  WhenChangedTo<TState>(
            this Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinition<TState>> smInfo,
            Func<Task> action) where TState : struct
        {
            if (smInfo?.Item3 == null) return null;
            var stateDefinition = smInfo.Item3;

            "Adding Behaviour: ChangedTo".Log();
            stateDefinition.ChangedTo = action;
            return smInfo;
        }


        public static ITransitionDefinition<TState> From<TState>(this ITransitionDefinition<TState> transition,
            TState state) where TState : struct
        {
            $"Defining start state {state}".Log();
            transition.StartState = state;
            return transition;
        }

        public static ITransitionDefinition<TState> To<TState>(this ITransitionDefinition<TState> transition,
            TState state) where TState : struct
        {
            $"Defining end state {state}".Log();
            transition.EndState = state;
            return transition;
        }

    }

    
}