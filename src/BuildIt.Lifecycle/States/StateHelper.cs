using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BuildIt.Lifecycle.States
{
    public static class StateHelper
    {
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
            stateDefinition.ChangedTo= action;
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
            transition.EndState= state;
            return transition;
        }

      
    }
}