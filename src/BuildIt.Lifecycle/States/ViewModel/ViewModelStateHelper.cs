using System;
using System.ComponentModel;
using System.Threading.Tasks;
using BuildIt.States;

namespace BuildIt.Lifecycle.States.ViewModel
{
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
    
    public static IViewModelStateDefinition<TState, TViewModel> Initialise<TState, TViewModel>(
            this IViewModelStateDefinition<TState, TViewModel> stateDefinition,
            Action<TViewModel> action)
            where TState : struct
            where TViewModel : INotifyPropertyChanged

        {
#pragma warning disable 1998 // Convert sync method into async call
            return stateDefinition.Initialise(async vm => action(vm));
#pragma warning restore 1998
        }

        public static IViewModelStateDefinition<TState, TViewModel> Initialise<TState, TViewModel>(
            this IViewModelStateDefinition<TState, TViewModel> stateDefinition,
            Func<TViewModel, Task> action)
            where TState : struct
            where TViewModel : INotifyPropertyChanged

        {
            if (stateDefinition == null) return null;

            "Adding Initialization".Log();
            stateDefinition.InitialiseViewModel = action;
            return stateDefinition;
        }

        public static IViewModelStateDefinition<TState, TViewModel> WhenAboutToChange<TState, TViewModel>(
            this IViewModelStateDefinition<TState, TViewModel> stateDefinition,
            Action<TViewModel,CancelEventArgs> action) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
#pragma warning disable 1998 // Convert sync method into async call
            return stateDefinition.WhenAboutToChange(async (vm,cancel) => action(vm,cancel));
#pragma warning restore 1998
        }

        public static IViewModelStateDefinition<TState, TViewModel> WhenAboutToChange<TState, TViewModel>(
            this IViewModelStateDefinition<TState, TViewModel> stateDefinition,
            Func<TViewModel, CancelEventArgs, Task> action) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
            if (stateDefinition == null) return null;

            "Adding Behaviour: AboutToChangeFromViewModel".Log();
            stateDefinition.AboutToChangeFromViewModel = action;
            return stateDefinition;
        }

        public static IViewModelStateDefinition<TState, TViewModel> WhenChangingFrom<TState, TViewModel>(
            this IViewModelStateDefinition<TState, TViewModel> stateDefinition,
            Action<TViewModel> action) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
#pragma warning disable 1998  // Convert sync method into async call
            return stateDefinition.WhenChangingFrom(async vm => action(vm));
#pragma warning restore 1998
        }

        public static IViewModelStateDefinition<TState, TViewModel> WhenChangingFrom<TState, TViewModel>(
            this IViewModelStateDefinition<TState, TViewModel> stateDefinition,
            Func<TViewModel, Task> action) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
            if (stateDefinition == null) return null;

            "Adding Behaviour: ChangingFromViewModel".Log();
            stateDefinition.ChangingFromViewModel = action;
            return stateDefinition;
        }

        public static IViewModelStateDefinition<TState, TViewModel> WhenChangedTo<TState, TViewModel>(
            this IViewModelStateDefinition<TState, TViewModel> stateDefinition,
            Action<TViewModel> action) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
#pragma warning disable 1998  // Convert sync method into async call
            return stateDefinition.WhenChangedTo(async vm=> action(vm));
#pragma warning restore 1998
        }

        public static IViewModelStateDefinition<TState, TViewModel> WhenChangedTo<TState, TViewModel>(
            this IViewModelStateDefinition<TState, TViewModel> stateDefinition,
            Func<TViewModel, Task> action) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
            if (stateDefinition == null) return null;

            "Adding Behaviour: ChangedToViewModel".Log();
            stateDefinition.ChangedToViewModel = action;
            return stateDefinition;
        }
    }
}