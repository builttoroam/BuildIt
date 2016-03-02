using System;
using System.ComponentModel;
using System.Threading.Tasks;
using BuildIt.States;
using BuildIt;
using BuildIt.States.Completion;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public static class ViewModelStateHelpers
    {
        //#region Create State Group

        //public static Tuple<IStateManager, IStateGroup<TState>> GroupWithViewModels<TState>(
        //   this IStateManager vsm) where TState : struct
        //{
        //    var grp = new StateGroup<TState>() as IStateGroup<TState>;
        //    vsm.AddStateGroup(grp);
        //    return TupleHelpers.Build(vsm, grp);
        //}

        //public static Tuple<IStateManager, IStateGroup<TState>> GroupWithViewModels<TState>(
        //    this Tuple<IStateManager, IStateGroup<TState>> vsmGroup)
        //    where TState : struct
        //{
        //    return vsmGroup.Item1.GroupWithViewModels<TState>();
        //}
        //#endregion

        #region Create State
        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinitionWithData<TState, TViewModel>>
            StateWithViewModel<TState, TViewModel>(
                this Tuple<IStateManager,
                    IStateGroup<TState>> smInfo,
                TState state)
            where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
            var vms = smInfo.Item2.DefineStateWithData<TViewModel>(state);
            return smInfo.Extend(vms);
        }

        public static Tuple<IStateManager, IStateGroup<TState>>
            EndState<TState, TViewModel>(this Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>> smInfo)
            where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
            return smInfo.Remove();
        }

        #endregion

        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinition<TState>> AsState<TState, TViewModel>(
            this Tuple<IStateManager,
                IStateGroup<TState>,
                IStateDefinitionWithData<TState, TViewModel>> smInfo)
            where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
            return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinition<TState>>(
                smInfo.Item1, smInfo.Item2, smInfo.Item3.State
                );
        }

        public static Tuple<IStateManager,
                IStateGroup<TState>,
                IStateDefinitionWithData<TState, TViewModel>> AsStateWithViewModel<TState, TViewModel>(
            this Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinition<TState>> smInfo)
            where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
            return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>>(
                smInfo.Item1, smInfo.Item2, new StateDefinitionWithDataWrapper<TState, TViewModel> { State = smInfo.Item3 }
                );
        }

        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinitionWithData<TState, TViewModel>> Initialise<TState, TViewModel>(
                this Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinitionWithData<TState, TViewModel>> smInfo,
                Action<TViewModel> action)
                where TState : struct
                where TViewModel : INotifyPropertyChanged

        {
#pragma warning disable 1998 // Convert sync method into async call
            return smInfo.Initialise(async vm => action(vm));
#pragma warning restore 1998
        }

        public static Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>> Initialise<TState, TViewModel>(
            this Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinitionWithData<TState, TViewModel>> smInfo,
            Func<TViewModel, Task> action)
            where TState : struct
            where TViewModel : INotifyPropertyChanged

        {
            var stateDefinition = smInfo?.Item3?.StateDataWrapper;
            if (stateDefinition == null) return null;

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

        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinitionWithData<TState, TViewModel>> WhenAboutToChange<TState, TViewModel>(
            this Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinitionWithData<TState, TViewModel>> stateDefinition,
            Action<TViewModel, CancelEventArgs> action) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
#pragma warning disable 1998 // Convert sync method into async call
            return stateDefinition.WhenAboutToChange(async (vm, cancel) => action(vm, cancel));
#pragma warning restore 1998
        }

        public static Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>> WhenAboutToChange<TState, TViewModel>(
            this Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>> smInfo,
            Func<TViewModel, CancelEventArgs, Task> action) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
            var stateDefinition = smInfo?.Item3?.StateDataWrapper;
            if (stateDefinition == null) return null;

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

        public static Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>> WhenChangingFrom<TState, TViewModel>(
            this Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>> smInfo,
            Action<TViewModel> action) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
#pragma warning disable 1998  // Convert sync method into async call
            return smInfo.WhenChangingFrom(async vm => action(vm));
#pragma warning restore 1998
        }

        public static Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>> WhenChangingFrom<TState, TViewModel>(
            this Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>> smInfo,
            Func<TViewModel, Task> action) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
            var stateDefinition = smInfo?.Item3?.StateDataWrapper;
            if (stateDefinition == null) return null;

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

        public static Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>> WhenChangedTo<TState, TViewModel>(
            this Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>> smInfo,
            Action<TViewModel> action) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
#pragma warning disable 1998  // Convert sync method into async call
            return smInfo.WhenChangedTo(async vm => action(vm));
#pragma warning restore 1998
        }

        public static Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>> WhenChangedTo<TState, TViewModel>(
            this Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>> smInfo,
            Func<TViewModel, Task> action) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
            var stateDefinition = smInfo?.Item3?.StateDataWrapper;
            if (stateDefinition == null) return null;

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

        public static Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>> WhenChangedToWithData<TState, TViewModel, TData>(
            this Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>> smInfo,
            Action<TViewModel, TData> action) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
#pragma warning disable 1998  // Convert sync method into async call
            return smInfo.WhenChangedToWithData<TState, TViewModel, TData>(async (vm, s) => action(vm, s));
#pragma warning restore 1998
        }

        public static Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>> WhenChangedToWithData<TState, TViewModel, TData>(
            this Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>> smInfo,
            Func<TViewModel, TData, Task> action) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
            var stateDefinition = smInfo?.Item3?.StateDataWrapper;
            if (stateDefinition == null) return null;

            "Adding Behaviour: ChangedToWithDataViewModel".Log();

            var modAction = new Func<TViewModel, string, Task>((vm, d) =>
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

        public static Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>, ViewModelStateEventBinder<TViewModel>>
            OnEvent<TState, TViewModel>(
            this Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>> smInfo,
            Action<TViewModel, EventHandler> subscribe,
            Action<TViewModel, EventHandler> unsubscribe)
            where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
            if (smInfo?.Item1 == null || smInfo.Item2 == null) return null;

            var binder = new ViewModelStateEventBinder<TViewModel> { Subscribe = subscribe, Unsubscribe = unsubscribe };
            return smInfo.Extend(binder);

            //new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>, StateEventBinder<TViewModel>>(
            //smInfo.Item1, smInfo.Item2, smInfo.Item3, binder);
            //"Adding Behaviour: ChangedToViewModel".Log();
            //stateDefinition.ChangedToViewModel = action;
            //return stateDefinition;
        }


        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinitionWithData<TState, TViewModel>, StateHelpers.StateCompletionBinder<DefaultCompletion>>
           OnDefaultComplete<TState, TViewModel>(
           this Tuple<IStateManager,
               IStateGroup<TState>,
               IStateDefinitionWithData<TState, TViewModel>> smInfo)
           where TState : struct
           where TViewModel : INotifyPropertyChanged, IViewModelCompletion<DefaultCompletion>
        {
            if (smInfo?.Item1 == null || smInfo.Item2 == null) return null;


            var binder = new StateHelpers.StateCompletionBinder<DefaultCompletion> { Completion = DefaultCompletion.Complete };
            return smInfo.Extend(binder);
        }


        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinitionWithData<TState, TViewModel>, StateHelpers.StateCompletionBinder<TCompletion>>
           OnComplete<TState, TViewModel, TCompletion>(
           this Tuple<IStateManager,
               IStateGroup<TState>,
               IStateDefinitionWithData<TState, TViewModel>> smInfo,
                TCompletion completion)
           where TState : struct
           where TViewModel : INotifyPropertyChanged, IViewModelCompletion<TCompletion>
            where TCompletion : struct
        {
            if (smInfo?.Item1 == null || smInfo.Item2 == null) return null;


            var binder = new StateHelpers.StateCompletionBinder<TCompletion> { Completion = completion };
            return smInfo.Extend(binder);
        }

        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinitionWithData<TState, TViewModel>,
            ViewModelStateCompletionWithDataBinder<TViewModel, DefaultCompletion, TData>>
           OnDefaultCompleteWithData<TState, TViewModel, TData>(
           this Tuple<IStateManager,
               IStateGroup<TState>,
               IStateDefinitionWithData<TState, TViewModel>> smInfo,
                Func<TViewModel, TData> completionData)
           where TState : struct
           where TViewModel : INotifyPropertyChanged, IViewModelCompletion<DefaultCompletion>
        {
            if (smInfo?.Item1 == null || smInfo.Item2 == null) return null;


            var binder = new ViewModelStateCompletionWithDataBinder<TViewModel, DefaultCompletion, TData> { Completion = DefaultCompletion.Complete, CompletionData = completionData };
            return smInfo.Extend(binder);

        }

        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinitionWithData<TState, TViewModel>,
            ViewModelStateCompletionWithDataBinder<TViewModel, TCompletion, TData>>
           OnCompleteWithData<TState, TViewModel, TCompletion, TData>(
           this Tuple<IStateManager,
               IStateGroup<TState>,
               IStateDefinitionWithData<TState, TViewModel>> smInfo,
                TCompletion completion,
                Func<TViewModel, TData> completionData)
           where TState : struct
           where TViewModel : INotifyPropertyChanged, IViewModelCompletion<TCompletion>
            where TCompletion : struct
        {
            if (smInfo?.Item1 == null || smInfo.Item2 == null) return null;


            var binder = new ViewModelStateCompletionWithDataBinder<TViewModel, TCompletion, TData> { Completion = completion, CompletionData = completionData };
            return smInfo.Extend(binder);

        }

        public static Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>> ChangeState<TState, TViewModel>(
            this Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>, ViewModelStateEventBinder<TViewModel>> smInfo,
            TState stateToChangeTo) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
            if (smInfo?.Item1 == null ||
                smInfo.Item2 == null ||
                smInfo.Item3 == null) return null;

            var binder = smInfo.Item4;
            var sm = smInfo.Item1;
            var newState = stateToChangeTo;
            var changeStateAction = new EventHandler((s, e) =>
            {
                sm.GoToState(newState);
            });

            var smreturn = smInfo.Remove();

            smreturn.WhenChangedTo(vm =>
                    {
                        binder.Subscribe(vm, changeStateAction);
                    })
                .WhenChangingFrom(vm =>
                    {
                        binder.Unsubscribe(vm, changeStateAction);
                    });

            return smreturn;

            //new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }

        public static Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>>
            ChangeToPreviousState<TState, TViewModel>(
            this Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>, ViewModelStateEventBinder<TViewModel>> smInfo
            ) where TState : struct
            where TViewModel : INotifyPropertyChanged
        {
            if (smInfo?.Item1 == null ||
                smInfo.Item2 == null ||
                smInfo.Item3 == null) return null;

            var binder = smInfo.Item4;
            var sm = smInfo.Item1;
            var changeStateAction = new EventHandler((s, e) =>
            {
                sm.GoBackToPreviousState();
            });

            var smreturn = smInfo.Remove();

            smreturn.WhenChangedTo(vm =>
                {
                    binder.Subscribe(vm, changeStateAction);
                })
                .WhenChangingFrom(vm =>
                {
                    binder.Unsubscribe(vm, changeStateAction);
                });
            return smreturn;
            //            return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }

        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinitionWithData<TState, TViewModel>>
            ChangeState<TState, TViewModel, TCompletion>(
            this Tuple<IStateManager, IStateGroup<TState>,
                IStateDefinitionWithData<TState, TViewModel>, StateHelpers.StateCompletionBinder<TCompletion>> smInfo,
            TState stateToChangeTo)
            where TState : struct
            where TViewModel : INotifyPropertyChanged, IViewModelCompletion<TCompletion>
            where TCompletion : struct
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

            var smreturn = smInfo.Remove();

            smreturn.WhenChangedTo(vm =>
                {
                    vm.Complete += changeStateAction;
                })
                .WhenChangingFrom(vm =>
                {
                    vm.Complete -= changeStateAction;
                });
            return smreturn;
            //            return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }

        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinitionWithData<TState, TViewModel>>
            CloseRegion<TState, TViewModel, TCompletion>(
            this Tuple<IStateManager, IStateGroup<TState>,
                IStateDefinitionWithData<TState, TViewModel>, StateHelpers.StateCompletionBinder<TCompletion>> smInfo,
            IApplicationRegion region)
            where TState : struct
            where TViewModel : INotifyPropertyChanged, IViewModelCompletion<TCompletion>
            where TCompletion : struct
        {
            if (smInfo?.Item1 == null ||
                smInfo.Item2 == null ||
                smInfo.Item3 == null) return null;

            var comp = smInfo.Item4?.Completion;
            var changeStateAction = new EventHandler<CompletionEventArgs<TCompletion>>(async (s, e) =>
            {
                if (e.Completion.Equals(comp))
                {
                    await region.RequestClose();
                }
            });

            var smreturn = smInfo.Remove();

            smreturn.WhenChangedTo(vm =>
            {
                vm.Complete += changeStateAction;
            })
                .WhenChangingFrom(vm =>
                {
                    vm.Complete -= changeStateAction;
                });
            return smreturn;
            //            return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }

        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinitionWithData<TState, TViewModel>>
            LaunchRegion<TState, TViewModel, TCompletion, TNewRegion>(
            this Tuple<IStateManager, IStateGroup<TState>,
                IStateDefinitionWithData<TState, TViewModel>, StateHelpers.StateCompletionBinder<TCompletion>> smInfo,
            IApplicationRegion region, TypeRef.TypeWrapper<TNewRegion> wrapper)
            where TState : struct
            where TViewModel : INotifyPropertyChanged, IViewModelCompletion<TCompletion>
            where TCompletion : struct
            where TNewRegion : IApplicationRegion
        {
            if (smInfo?.Item1 == null ||
                smInfo.Item2 == null ||
                smInfo.Item3 == null) return null;

            var comp = smInfo.Item4?.Completion;
            var changeStateAction = new EventHandler<CompletionEventArgs<TCompletion>>((s, e) =>
           {
               if (e.Completion.Equals(comp))
               {
                   region.Manager.CreateRegion<TNewRegion>();
               }
           });

            var smreturn = smInfo.Remove();

            smreturn.WhenChangedTo(vm =>
            {
                vm.Complete += changeStateAction;
            })
                .WhenChangingFrom(vm =>
                {
                    vm.Complete -= changeStateAction;
                });
            return smreturn;
            //            return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }

        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinitionWithData<TState, TViewModel>>
            ChangeState<TState, TViewModel, TCompletion, TData>(
            this Tuple<IStateManager, IStateGroup<TState>,
                IStateDefinitionWithData<TState, TViewModel>,
                ViewModelStateCompletionWithDataBinder<TViewModel, TCompletion, TData>> smInfo,
            TState stateToChangeTo)
            where TState : struct
            where TViewModel : INotifyPropertyChanged, IViewModelCompletion<TCompletion>
            where TCompletion : struct
        {
            if (smInfo?.Item1 == null ||
                smInfo.Item2 == null ||
                smInfo.Item3 == null) return null;

            var comp = smInfo.Item4?.Completion;
            var data = smInfo.Item4?.CompletionData;
            var sm = smInfo.Item1;
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
                    var vvm = (TViewModel)s;
                    dataVal = data(vvm);
                }
                if (e.Completion.Equals(comp))
                {
                    sm.GoToStateWithData(newState, dataVal);
                }
            });



            var smreturn = smInfo.Remove();

            smreturn.WhenChangedTo(vm =>
                {
                    vm.Complete += changeStateAction;
                })
                .WhenChangingFrom(vm =>
                {
                    vm.Complete -= changeStateAction;
                });
            return smreturn;
            //            return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }
        public static Tuple<IStateManager,
            IStateGroup<TState>,
            IStateDefinitionWithData<TState, TViewModel>>
            ChangeToPreviousState<TState, TViewModel, TCompletion>(
            this Tuple<IStateManager, IStateGroup<TState>,
                IStateDefinitionWithData<TState, TViewModel>, StateHelpers.StateCompletionBinder<TCompletion>> smInfo
            )
            where TState : struct
            where TViewModel : INotifyPropertyChanged, IViewModelCompletion<TCompletion>
            where TCompletion : struct
        {
            if (smInfo?.Item1 == null ||
                smInfo.Item2 == null ||
                smInfo.Item3 == null) return null;

            var comp = smInfo.Item4?.Completion;
            var sm = smInfo.Item1;
            var changeStateAction = new EventHandler<CompletionEventArgs<TCompletion>>((s, e) =>
            {
                if (e.Completion.Equals(comp))
                {
                    sm.GoBackToPreviousState();
                }
            });



            var smreturn = smInfo.Remove();

            smreturn.WhenChangedTo(vm =>
                {
                    vm.Complete += changeStateAction;
                })
                .WhenChangingFrom(vm =>
                {
                    vm.Complete -= changeStateAction;
                });
            return smreturn;
            //            return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TViewModel>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }

        public class ViewModelStateEventBinder<TViewModel> where TViewModel : INotifyPropertyChanged
        {
            public Action<TViewModel, EventHandler> Subscribe { get; set; }
            public Action<TViewModel, EventHandler> Unsubscribe { get; set; }
        }



        public class ViewModelStateCompletionWithDataBinder<TViewModel, TCompletion, TData> : StateHelpers.StateCompletionBinder<TCompletion>
            where TCompletion : struct
            where TViewModel : INotifyPropertyChanged, IViewModelCompletion<TCompletion>
        {

            public Func<TViewModel, TData> CompletionData { get; set; }

        }


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