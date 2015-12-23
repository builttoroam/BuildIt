using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BuildIt.Lifecycle.States.ViewModel;

namespace BuildIt.Lifecycle.States
{
    public static class StateHelpers
    {
        public static Tuple<IStateManager, StateGroup<TState, DefaultTransition>> Group<TState>(
            this IStateManager vsm) where TState : struct
        {
            return vsm.GroupWithTransition<TState, DefaultTransition>();
            //var grp = new StateGroup<TState, DefaultTransition>();
            //vsm.StateGroups.Add(typeof(TState), grp);
            //return new Tuple<IStateManager, StateGroup<TState, DefaultTransition>>(vsm, grp);
        }

        public static Tuple<IStateManager, StateGroup<TState, DefaultTransition>> Group<TState>(
            this Tuple<IStateManager, StateGroup<TState, DefaultTransition>> vsmGroup)
            where TState : struct
        {
            return vsmGroup.Item1.Group<TState>();
        }

        public static Tuple<IStateManager, StateGroup<TState, TTransition>> GroupWithTransition<TState, TTransition>(
            this IStateManager vsm) where TState : struct
            where TTransition : struct
        {
            var grp = new StateGroup<TState, TTransition>();
            vsm.StateGroups.Add(typeof (TState), grp);
            return new Tuple<IStateManager, StateGroup<TState, TTransition>>(vsm, grp);
        }

        public static Tuple<IStateManager, StateGroup<TState, TTransition>> GroupWithTransition<TState, TTransition>(
            this Tuple<IStateManager, StateGroup<TState, TTransition>> vsmGroup)
            where TState : struct
            where TTransition : struct
        {
            return vsmGroup.Item1.GroupWithTransition<TState, TTransition>();
        }

        public static Tuple<IStateManager,
            StateGroup<TState, DefaultTransition>> Group<TState, TExistingState>(
            this Tuple<IStateManager, StateGroup<TExistingState, DefaultTransition>,
                StateDefinition<TExistingState>> vsmGroup, TState state)
            where TExistingState : struct
            where TState : struct
        {
            var grp = new StateGroup<TState, DefaultTransition>();
            vsmGroup.Item1.StateGroups.Add(typeof(TState), grp);
            return new Tuple<IStateManager, StateGroup<TState, DefaultTransition>>(vsmGroup.Item1, grp);
        }


        public static Tuple<IStateManager,
            StateGroup<TState, DefaultTransition>> Group<TState, TExistingState, TExistingElement, TExistingPropertyValue>(
            this Tuple<IStateManager, StateGroup<TExistingState, DefaultTransition>,
                StateDefinition<TExistingState>,
                StateValue<TExistingElement, TExistingPropertyValue>> vsmGroup, TState state)
            where TExistingState : struct
            where TState : struct
        {
            var grp = new StateGroup<TState, DefaultTransition>();
            vsmGroup.Item1.StateGroups.Add(typeof(TState), grp);
            return new Tuple<IStateManager, StateGroup<TState, DefaultTransition>>(vsmGroup.Item1, grp);
        }

        public static Tuple<IStateManager, StateGroup<TState, DefaultTransition>,
            StateDefinition<TState>> DefineState<TState>(
            this Tuple<IStateManager, StateGroup<TState, DefaultTransition>> vsmGroup, TState state)
            where TState : struct
        {
            var vs = new StateDefinition<TState> { State = state };
            vsmGroup.Item2.States.Add(state, vs);
            return new Tuple<IStateManager, StateGroup<TState, DefaultTransition>, StateDefinition<TState>>(vsmGroup.Item1, vsmGroup.Item2, vs);
        }

        public static Tuple<IStateManager, StateGroup<TState, DefaultTransition>, StateDefinition<TState>> DefineState<TState>(
            this Tuple<IStateManager, StateGroup<TState, DefaultTransition>, StateDefinition<TState>> vsmGroup, TState state) where TState : struct
        {
            var vs = new StateDefinition<TState> { State = state };
            vsmGroup.Item2.States.Add(state, vs);
            return new Tuple<IStateManager, StateGroup<TState, DefaultTransition>, StateDefinition<TState>>(vsmGroup.Item1, vsmGroup.Item2, vs);
        }

        public static Tuple<IStateManager,
            StateGroup<TState, DefaultTransition>, StateDefinition<TState>> DefineState<TState, TExistingState, TExistingElement, TExistingPropertyValue>(
            this Tuple<IStateManager, StateGroup<TState, DefaultTransition>, StateDefinition<TExistingState>, StateValue<TExistingElement, TExistingPropertyValue>> vsmGroup, TState state)
            where TExistingState : struct
            where TState : struct
        {
            var vs = new StateDefinition<TState> { State = state };
            vsmGroup.Item2.States.Add(state, vs);
            return new Tuple<IStateManager,
                StateGroup<TState, DefaultTransition>, StateDefinition<TState>>(vsmGroup.Item1, vsmGroup.Item2, vs);
        }


        public static Tuple<IStateManager,
            StateGroup<TState, DefaultTransition>, StateDefinition<TState>,
            TElement> Target<TState, TElement>(
            this Tuple<IStateManager, StateGroup<TState, DefaultTransition>, StateDefinition<TState>> vsmGroup, TElement element) where TState : struct
        {
            return new Tuple<IStateManager, StateGroup<TState, DefaultTransition>, StateDefinition<TState>, TElement>(vsmGroup.Item1, vsmGroup.Item2, vsmGroup.Item3, element);
        }

        public static Tuple<IStateManager,
            StateGroup<TState, DefaultTransition>, StateDefinition<TState>,
            TElement> Target<TState, TElement, TExistingElement, TExistingPropertyValue>(
            this Tuple<IStateManager, StateGroup<TState, DefaultTransition>, StateDefinition<TState>,
                StateValue<TExistingElement, TExistingPropertyValue>> vsmGroup, TElement element)
            where TState : struct

        {
            return new Tuple<IStateManager,
                StateGroup<TState, DefaultTransition>, StateDefinition<TState>,
                TElement>(vsmGroup.Item1, vsmGroup.Item2, vsmGroup.Item3, element);
        }


        public static Tuple<IStateManager,
            StateGroup<TState, DefaultTransition>, StateDefinition<TState>, StateValue<TElement, TPropertyValue>> Change<TState, TElement, TPropertyValue>(
            this Tuple<IStateManager,
                StateGroup<TState, DefaultTransition>, StateDefinition<TState>,
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
            return new Tuple<IStateManager, StateGroup<TState, DefaultTransition>, StateDefinition<TState>, StateValue<TElement, TPropertyValue>>(
                vsmGroup.Item1, vsmGroup.Item2, vsmGroup.Item3, vsv);
        }

        public static Tuple<IStateManager, StateGroup<TState, DefaultTransition>, StateDefinition<TState>,
            StateValue<TElement, TPropertyValue>> ToValue<TState, TElement, TPropertyValue>(
            this Tuple<IStateManager, StateGroup<TState, DefaultTransition>, StateDefinition<TState>,
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

        public static IStateDefinition<TState> WhenAboutToChange<TState>(
           this IStateDefinition<TState> stateDefinition,
           Action<CancelEventArgs> action) where TState : struct
        {
#pragma warning disable 1998  // Convert sync method into async call
            return stateDefinition.WhenAboutToChange(async cancel => action(cancel));
#pragma warning restore 1998
        }

        public static IStateDefinition<TState> WhenAboutToChange<TState>(
            this IStateDefinition<TState> stateDefinition,
            Func<CancelEventArgs, Task> action) where TState : struct
        {
            if (stateDefinition == null) return null;

            "Adding Behaviour: AboutToChangeFrom".Log();
            stateDefinition.AboutToChangeFrom = action;
            return stateDefinition;
        }

        public static IStateDefinition<TState> WhenChangingFrom<TState>(
            this IStateDefinition<TState> stateDefinition,
            Action action) where TState : struct
        {
#pragma warning disable 1998  // Convert sync method into async call
            return stateDefinition.WhenChangingFrom(async () => action());
#pragma warning restore 1998
        }

        public static IStateDefinition<TState> WhenChangingFrom<TState>(
            this IStateDefinition<TState> stateDefinition,
            Func<Task> action) where TState : struct
        {
            if (stateDefinition == null) return null;

            "Adding Behaviour: ChangingFrom".Log();
            stateDefinition.ChangingFrom = action;
            return stateDefinition;
        }

        public static IStateDefinition<TState> WhenChangedTo<TState>(
            this IStateDefinition<TState> stateDefinition,
            Action action) where TState : struct
        {
#pragma warning disable 1998  // Convert sync method into async call
            return stateDefinition.WhenChangedTo(async () => action());
#pragma warning restore 1998
        }

        public static IStateDefinition<TState> WhenChangedTo<TState>(
            this IStateDefinition<TState> stateDefinition,
            Func<Task> action) where TState : struct
        {
            if (stateDefinition == null) return null;

            "Adding Behaviour: ChangedTo".Log();
            stateDefinition.ChangedTo = action;
            return stateDefinition;
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

    public static class ViewModelStateHelpers
    {
        public static Tuple<IStateManager, ViewModelStateGroup<TState, DefaultTransition>> GroupWithViewModels<TState>(
           this IStateManager vsm) where TState : struct
        {
            var grp = new ViewModelStateGroup<TState, DefaultTransition>();
            vsm.StateGroups.Add(typeof(TState), grp);
            return new Tuple<IStateManager, ViewModelStateGroup<TState, DefaultTransition>>(vsm, grp);
        }

        public static Tuple<IStateManager, ViewModelStateGroup<TState, DefaultTransition>> Group<TState>(
            this Tuple<IStateManager, ViewModelStateGroup<TState, DefaultTransition>> vsmGroup)
            where TState : struct
        {
            return vsmGroup.Item1.GroupWithViewModels<TState>();
        }
    }
}