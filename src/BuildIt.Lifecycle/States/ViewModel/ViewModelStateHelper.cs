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



        public static Tuple<IStateManager, ViewModelStateGroup<TState, DefaultTransition>,IViewModelStateDefinition<TState, TViewModel>>
            StateWithViewModel<TState, TViewModel>(this Tuple<IStateManager, ViewModelStateGroup<TState, DefaultTransition>>  smInfo,
            TState state)
            where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
            //var gp = sm.StateGroups[typeof(TState)] as IViewModelStateGroup<TState>;
            var vms = smInfo.Item2.DefineViewModelState<TViewModel>(state);
            return new Tuple<IStateManager, ViewModelStateGroup<TState, DefaultTransition>,
                IViewModelStateDefinition<TState, TViewModel>>(smInfo.Item1,smInfo.Item2, vms);
        }

        public static Tuple<IStateManager, ViewModelStateGroup<TState, DefaultTransition>>
            EndState<TState, TViewModel>(this Tuple<IStateManager, ViewModelStateGroup<TState, DefaultTransition>, IViewModelStateDefinition<TState, TViewModel>> smInfo)
            where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
            //var gp = sm.StateGroups[typeof(TState)] as IViewModelStateGroup<TState>;
            return new Tuple<IStateManager, ViewModelStateGroup<TState, DefaultTransition>>(smInfo.Item1, smInfo.Item2);
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
            if (stateDefinition.InitialiseViewModel == null || action == null)
            {
                stateDefinition.InitialiseViewModel = action;
            }
            else
            {
                stateDefinition.InitialiseViewModel += action;
            }
            return stateDefinition;
        }

        public static IViewModelStateDefinition<TState, TViewModel> WhenAboutToChange<TState, TViewModel>(
            this IViewModelStateDefinition<TState, TViewModel> stateDefinition,
            Action<TViewModel, CancelEventArgs> action) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
#pragma warning disable 1998 // Convert sync method into async call
            return stateDefinition.WhenAboutToChange(async (vm, cancel) => action(vm, cancel));
#pragma warning restore 1998
        }

        public static IViewModelStateDefinition<TState, TViewModel> WhenAboutToChange<TState, TViewModel>(
            this IViewModelStateDefinition<TState, TViewModel> stateDefinition,
            Func<TViewModel, CancelEventArgs, Task> action) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
            if (stateDefinition == null) return null;

            "Adding Behaviour: AboutToChangeFromViewModel".Log();
            if (stateDefinition.AboutToChangeFromViewModel == null || action == null)
            {
                stateDefinition.AboutToChangeFromViewModel = action;
            }
            else
            {
                stateDefinition.AboutToChangeFromViewModel += action;
            }
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
            if (stateDefinition.ChangingFromViewModel == null || action == null)
            {
                stateDefinition.ChangingFromViewModel = action;
            }
            else
            {
                stateDefinition.ChangingFromViewModel += action;
            }
            return stateDefinition;
        }

        public static IViewModelStateDefinition<TState, TViewModel> WhenChangedTo<TState, TViewModel>(
            this IViewModelStateDefinition<TState, TViewModel> stateDefinition,
            Action<TViewModel> action) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
#pragma warning disable 1998  // Convert sync method into async call
            return stateDefinition.WhenChangedTo(async vm => action(vm));
#pragma warning restore 1998
        }

        public static IViewModelStateDefinition<TState, TViewModel> WhenChangedTo<TState, TViewModel>(
            this IViewModelStateDefinition<TState, TViewModel> stateDefinition,
            Func<TViewModel, Task> action) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
            if (stateDefinition == null) return null;

            "Adding Behaviour: ChangedToViewModel".Log();

            if (stateDefinition.ChangedToViewModel == null || action == null)
            {
                stateDefinition.ChangedToViewModel = action;
            }
            else
            {
                stateDefinition.ChangedToViewModel += action;
            }
            return stateDefinition;
        }

        public static Tuple<IStateManager, ViewModelStateGroup<TState, DefaultTransition>, IViewModelStateDefinition<TState, TViewModel>, StateEventBinder<TViewModel>> 
            OnEvent<TState, TViewModel>(
            this Tuple<IStateManager, ViewModelStateGroup<TState, DefaultTransition>, IViewModelStateDefinition<TState, TViewModel>> smInfo,
            Action<TViewModel, EventHandler> subscribe,
            Action<TViewModel, EventHandler> unsubscribe) 
            where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
            if (smInfo?.Item1 == null || smInfo.Item2 == null) return null;

            var binder = new StateEventBinder<TViewModel> { Subscribe = subscribe, Unsubscribe = unsubscribe };
            return new Tuple<IStateManager, ViewModelStateGroup<TState, DefaultTransition>, IViewModelStateDefinition<TState, TViewModel>, StateEventBinder <TViewModel>>(
                smInfo.Item1, smInfo.Item2,smInfo.Item3, binder);
            //"Adding Behaviour: ChangedToViewModel".Log();
            //stateDefinition.ChangedToViewModel = action;
            //return stateDefinition;
        }

      


        public static Tuple<IStateManager, 
            ViewModelStateGroup<TState, DefaultTransition>, 
            IViewModelStateDefinition<TState, TViewModel>, 
            StateCompletionBinder<TCompletion>>
           OnComplete<TState, TViewModel, TCompletion>(
           this Tuple<IStateManager, 
               ViewModelStateGroup<TState, DefaultTransition>, 
               IViewModelStateDefinition<TState, TViewModel>> smInfo,
                TCompletion completion)
           where TState : struct
           where TViewModel : INotifyPropertyChanged, IViewModelCompletion<TCompletion>
            where TCompletion:struct
        {
            if (smInfo?.Item1 == null || smInfo.Item2 == null) return null;


            var binder = new StateCompletionBinder<TCompletion> { Completion=completion };
            return new Tuple<IStateManager, 
                ViewModelStateGroup<TState, DefaultTransition>, 
                IViewModelStateDefinition<TState, TViewModel>, 
                StateCompletionBinder<TCompletion>>(
                smInfo.Item1, smInfo.Item2, smInfo.Item3, binder);
            //"Adding Behaviour: ChangedToViewModel".Log();
            //stateDefinition.ChangedToViewModel = action;
            //return stateDefinition;
        }

        public static Tuple<IStateManager, ViewModelStateGroup<TState, DefaultTransition>, IViewModelStateDefinition<TState, TViewModel>> ChangeState<TState, TViewModel>(
            this Tuple<IStateManager, ViewModelStateGroup<TState, DefaultTransition>, IViewModelStateDefinition<TState, TViewModel>, StateEventBinder<TViewModel>> smInfo,
            TState stateToChangeTo) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
            if (smInfo?.Item1 == null ||
                smInfo.Item2 == null ||
                smInfo.Item3 == null) return null;

            var binder = smInfo.Item4;
            var sm = smInfo.Item1;
            var newState = stateToChangeTo;
            var changeStateAction = new EventHandler((s,e) =>
            {
                sm.GoToState(newState);
            });

            smInfo.Item3
                .WhenChangedTo(vm =>
                    {
                        binder.Subscribe(vm, changeStateAction);
                    })
                .WhenChangingFrom(vm =>
                    {
                        binder.Unsubscribe(vm, changeStateAction);
                    });

            return new Tuple<IStateManager, ViewModelStateGroup<TState, DefaultTransition>, IViewModelStateDefinition<TState, TViewModel>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }
        public static Tuple<IStateManager, 
            ViewModelStateGroup<TState, DefaultTransition>, 
            IViewModelStateDefinition<TState, TViewModel>> 
            ChangeState<TState, TViewModel, TCompletion>(
            this Tuple<IStateManager, ViewModelStateGroup<TState, DefaultTransition>, 
                IViewModelStateDefinition<TState, TViewModel>, 
                StateCompletionBinder<TCompletion>> smInfo,
            TState stateToChangeTo) 
            where TState : struct
            where TViewModel : INotifyPropertyChanged,IViewModelCompletion<TCompletion>
            where TCompletion:struct
        {
            if (smInfo?.Item1 == null ||
                smInfo.Item2 == null ||
                smInfo.Item3 == null) return null;

            var comp = smInfo.Item4?.Completion;
            var sm = smInfo.Item1;
            var newState = stateToChangeTo;
            var changeStateAction = new EventHandler<CompletionEventArgs<TCompletion>>((s, e) =>
            {
                if (e.Completion.Equals(comp))
                {
                    sm.GoToState(newState);
                }
            });



            smInfo.Item3
                .WhenChangedTo(vm =>
                {
                    vm.Complete += changeStateAction;
                })
                .WhenChangingFrom(vm =>
                {
                    vm.Complete -= changeStateAction;
                });

            return new Tuple<IStateManager, ViewModelStateGroup<TState, DefaultTransition>, IViewModelStateDefinition<TState, TViewModel>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }

        public class StateEventBinder<TViewModel> where TViewModel : INotifyPropertyChanged
        {
            public Action<TViewModel, EventHandler> Subscribe { get; set; }
            public Action<TViewModel, EventHandler> Unsubscribe { get; set; }
        }

        public class StateCompletionBinder<TCompletion>
            where TCompletion : struct
        {
            public TCompletion Completion { get; set; }
        } 
    }
}