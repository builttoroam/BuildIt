using System;
using System.Linq.Expressions;

namespace BuildIt.VisualStates
{
    public static class StateHelpers
    {
        public static Tuple<IVisualStateManager, VisualStateGroup<TVisualState>> Group<TVisualState>(
            this IVisualStateManager vsm) where TVisualState : struct
        {
            var grp = new VisualStateGroup<TVisualState>();
            vsm.VisualStateGroups.Add(typeof(TVisualState), grp);
            return new Tuple<IVisualStateManager, VisualStateGroup<TVisualState>>(vsm, grp);
        }

        public static Tuple<IVisualStateManager, VisualStateGroup<TVisualState>> Group<TVisualState>(
            this Tuple<IVisualStateManager, VisualStateGroup<TVisualState>> vsmGroup) where TVisualState : struct
        {
            return vsmGroup.Item1.Group<TVisualState>();
        }

        public static Tuple<IVisualStateManager,
            VisualStateGroup<TVisualState>> Group<TVisualState, TExistingVisualState>(
            this Tuple<IVisualStateManager, VisualStateGroup<TExistingVisualState>, VisualState<TExistingVisualState>> vsmGroup, TVisualState state)
            where TExistingVisualState : struct
            where TVisualState : struct
        {
            var grp = new VisualStateGroup<TVisualState>();
            vsmGroup.Item1.VisualStateGroups.Add(typeof(TVisualState), grp);
            return new Tuple<IVisualStateManager, VisualStateGroup<TVisualState>>(vsmGroup.Item1, grp);
        }


        public static Tuple<IVisualStateManager,
            VisualStateGroup<TVisualState>> Group<TVisualState, TExistingVisualState, TExistingElement, TExistingPropertyValue>(
            this Tuple<IVisualStateManager, VisualStateGroup<TExistingVisualState>, VisualState<TExistingVisualState>,
                VisualStateValue<TExistingElement, TExistingPropertyValue>> vsmGroup, TVisualState state)
            where TExistingVisualState : struct
            where TVisualState : struct
        {
            var grp = new VisualStateGroup<TVisualState>();
            vsmGroup.Item1.VisualStateGroups.Add(typeof(TVisualState), grp);
            return new Tuple<IVisualStateManager, VisualStateGroup<TVisualState>>(vsmGroup.Item1, grp);
        }

        public static Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TVisualState>> DefineState<TVisualState>(
            this Tuple<IVisualStateManager, VisualStateGroup<TVisualState>> vsmGroup, TVisualState state) where TVisualState : struct
        {
            var vs = new VisualState<TVisualState>(state);
            vsmGroup.Item2.VisualStates.Add(state, vs);
            return new Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TVisualState>>(vsmGroup.Item1, vsmGroup.Item2, vs);
        }

        public static Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TVisualState>> DefineState<TVisualState>(
            this Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TVisualState>> vsmGroup, TVisualState state) where TVisualState : struct
        {
            var vs = new VisualState<TVisualState>(state);
            vsmGroup.Item2.VisualStates.Add(state, vs);
            return new Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TVisualState>>(vsmGroup.Item1, vsmGroup.Item2, vs);
        }

        public static Tuple<IVisualStateManager,
            VisualStateGroup<TVisualState>,
            VisualState<TVisualState>> DefineState<TVisualState, TExistingVisualState, TExistingElement, TExistingPropertyValue>(
            this Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TExistingVisualState>,
                VisualStateValue<TExistingElement, TExistingPropertyValue>> vsmGroup, TVisualState state)
            where TExistingVisualState : struct
            where TVisualState : struct
        {
            var vs = new VisualState<TVisualState>(state);
            vsmGroup.Item2.VisualStates.Add(state, vs);
            return new Tuple<IVisualStateManager,
                VisualStateGroup<TVisualState>,
                VisualState<TVisualState>>(vsmGroup.Item1, vsmGroup.Item2, vs);
        }


        public static Tuple<IVisualStateManager,
            VisualStateGroup<TVisualState>,
            VisualState<TVisualState>,
            TElement> Target<TVisualState, TElement>(
            this Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TVisualState>> vsmGroup, TElement element) where TVisualState : struct
        {
            return new Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TVisualState>, TElement>(vsmGroup.Item1, vsmGroup.Item2, vsmGroup.Item3, element);
        }

        public static Tuple<IVisualStateManager,
            VisualStateGroup<TVisualState>,
            VisualState<TVisualState>,
            TElement> Target<TVisualState, TElement, TExistingElement, TExistingPropertyValue>(
            this Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TVisualState>,
                VisualStateValue<TExistingElement, TExistingPropertyValue>> vsmGroup, TElement element)
            where TVisualState : struct

        {
            return new Tuple<IVisualStateManager,
                VisualStateGroup<TVisualState>,
                VisualState<TVisualState>,
                TElement>(vsmGroup.Item1, vsmGroup.Item2, vsmGroup.Item3, element);
        }


        public static Tuple<IVisualStateManager,
            VisualStateGroup<TVisualState>,
            VisualState<TVisualState>,
            VisualStateValue<TElement, TPropertyValue>> Change<TVisualState, TElement, TPropertyValue>(
            this Tuple<IVisualStateManager,
                VisualStateGroup<TVisualState>,
                VisualState<TVisualState>,
                TElement> vsmGroup,
            Expression<Func<TElement, TPropertyValue>> getter,
            Action<TElement, TPropertyValue> setter) where TVisualState : struct
        {
            var vsv = new VisualStateValue<TElement, TPropertyValue>
            {
                Key = new Tuple<object, string>(vsmGroup.Item4, (getter.Body as MemberExpression)?.Member.Name),
                Element = vsmGroup.Item4,
                Getter = getter.Compile(),
                Setter = setter
            };
            return new Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TVisualState>, VisualStateValue<TElement, TPropertyValue>>(
                vsmGroup.Item1, vsmGroup.Item2, vsmGroup.Item3, vsv);
        }

        public static Tuple<IVisualStateManager, VisualStateGroup<TVisualState>, VisualState<TVisualState>,
            VisualStateValue<TElement, TPropertyValue>> ToValue<TVisualState, TElement, TPropertyValue>(
            this Tuple<IVisualStateManager, VisualStateGroup<TVisualState>,
                VisualState<TVisualState>, VisualStateValue<TElement, TPropertyValue>> vsmGroup,
            TPropertyValue value) where TVisualState : struct
        {
            vsmGroup.Item4.StateValue = value;
            vsmGroup.Item3.Values.Add(vsmGroup.Item4);
            return vsmGroup;
        }

        public static VisualStateValue<TElement, TPropertyValue> ChangeProperty<TElement, TPropertyValue>(
            this TElement element, Expression<Func<TElement, TPropertyValue>> getter, Action<TElement, TPropertyValue> setter)

        {
            return new VisualStateValue<TElement, TPropertyValue>
            {
                Key = new Tuple<object, string>(element, (getter.Body as MemberExpression)?.Member.Name),
                Element = element,
                Getter = getter.Compile(),
                Setter = setter
            };
        }

        public static VisualStateValue<TElement, TPropertyValue> ToValue<TElement, TPropertyValue>(
            this VisualStateValue<TElement, TPropertyValue> state, TPropertyValue newValue)

        {
            state.StateValue = newValue;
            return state;
        }
    }
}