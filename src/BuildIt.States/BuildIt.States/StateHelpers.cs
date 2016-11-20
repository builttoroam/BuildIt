using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
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


    public interface IStateWithDataActionData<TState, TStateData,TData> :
        IStateWithDataAction<TState, TStateData>
        where TStateData : INotifyPropertyChanged
        where TState : struct
    {

    }

    public interface IStateDefinitionWithDataChangeStateWithDataBuilder<TState, TStateData, TNewStateData> :
        IStateDefinitionWithDataBuilder<TState, TStateData>
        where TStateData : INotifyPropertyChanged
        where TState : struct
    {
        TState NewState { get; }
    }

    public interface IStateWithDataAction<TState,TStateData>: 
        IStateDefinitionWithDataBuilder<TState, TStateData>
        where TStateData : INotifyPropertyChanged
        where TState : struct
    {
        Action<TStateData> WhenChangedToNewState(TState newState);
        Action<TStateData> WhenChangingFromNewState(TState newState);

        Action<TStateData> WhenChangedToPreviousState();
        Action<TStateData> WhenChangingFromPreviousState();
    }

    public interface IStateDefinitionWithDataEventBuilder<TState, TStateData> : 
        IStateWithDataAction<TState,TStateData>
        where TStateData : INotifyPropertyChanged
        where TState : struct
    {
        Action<TStateData, EventHandler> Subscribe { get; }
        Action<TStateData, EventHandler> Unsubscribe { get; }

    }



    public interface IStateDefinitionValueTargetBuilder<TState, TElement> : IStateDefinitionBuilder<TState>
where TState : struct
    {
        TElement Target { get; }
    }


    public interface IStateDefinitionValueBuilder<TState, TElement, TPropertyValue> : IStateDefinitionBuilder<TState>
    where TState : struct
    {
        StateValue<TElement, TPropertyValue> Value { get; }
    }

    public interface IStateCompletionBuilder<TState, TCompletion> : IStateDefinitionBuilder<TState>
        where TState : struct
        where TCompletion : struct
    {
        TCompletion Completion { get; }
    }

    public interface IStateCompletionWithDataBuilder<TState, TCompletion, TData> 
        : IStateCompletionBuilder<TState, TCompletion>
        where TState : struct
        where TCompletion : struct
    {
        Func<TData> Data { get; }
    }

    public interface IStateWithDataCompletionBuilder<TState, TStateData, TCompletion> :
        IStateCompletionBuilder<TState, TCompletion>,
        IStateWithDataAction<TState,TStateData> 
        where TState : struct
        where TCompletion : struct
        where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
    {
    }

    public interface IStateWithDataCompletionWithDataEventBuilder
        <TState, TStateData, TCompletion, TData> :
         IStateCompletionBuilder<TState, TCompletion>,
        IStateWithDataActionData<TState, TStateData,TData> 
        where TState : struct
        where TCompletion : struct
        where TStateData : INotifyPropertyChanged, ICompletionWithData<TCompletion,TData>
    {
    }

    public interface IStateWithDataCompletionWithDataBuilder<TState, TStateData, TCompletion, TData> :
        IStateWithDataCompletionBuilder<TState, TStateData, TCompletion>,
        IStateWithDataActionData<TState,TStateData,TData>
        where TState : struct
        where TCompletion : struct
        where TStateData : INotifyPropertyChanged,ICompletion<TCompletion>
    {
        Func<TStateData, TData> Data { get; }
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

        private class StateDefinitionWithDataBuilder<TState, TData> : StateGroupBuilder<TState>,
            IStateDefinitionWithDataBuilder<TState, TData>
            where TData : INotifyPropertyChanged
           where TState : struct
        {
            public IStateDefinition<TState> State { get; set; }
            public IStateDefinitionTypedDataWrapper<TData> StateDataWrapper
            => State.UntypedStateDataWrapper as IStateDefinitionTypedDataWrapper<TData>;
        }

        

            private class StateDefinitionWithDataChangeStateWithDataBuilder<TState, TStateData,TNewStateData> : 
             StateDefinitionWithDataBuilder<TState, TStateData>, IStateDefinitionWithDataChangeStateWithDataBuilder<TState, TStateData, TNewStateData>
            where TStateData : INotifyPropertyChanged
           where TState : struct
        {
            public TState NewState { get; set; }

        }

        private class EventHandlerCache<TState,TEventHandler>
            where TState : struct
        {
            private IDictionary<TState, TEventHandler> Handlers { get; } = new Dictionary<TState, TEventHandler>();

            public TEventHandler BuildHandler(TState newState, Func<TState, TEventHandler> createHandler)
            {
                var changeStateAction = Handlers.SafeValue(newState);
                if (changeStateAction == null)
                {

                    changeStateAction = createHandler(newState);
                    Handlers[newState] = changeStateAction;
                }
                return changeStateAction;
            }

        }

        private class StateDefinitionWithDataEventBuilder<TState, TData> : StateDefinitionWithDataBuilder<TState, TData>,
            IStateDefinitionWithDataEventBuilder<TState, TData>
            where TData : INotifyPropertyChanged
           where TState : struct
        {
            public Action<TData, EventHandler> Subscribe { get; set; }
            public Action<TData, EventHandler> Unsubscribe { get; set; }

            private EventHandlerCache<TState, EventHandler> HandlerCache { get; }= new EventHandlerCache<TState,EventHandler>();
            //private IDictionary<TState,EventHandler> Handlers { get; } = new Dictionary<TState,EventHandler>();

            public Action<TData> WhenChangedToNewState(TState newState)
            {
                var changeStateAction = HandlerCache.BuildHandler(newState,Create);

                return vm =>
                {
                    Subscribe(vm, changeStateAction);
                };
            }

            private EventHandler Create(TState newState)
            {
                EventHandler changeStateAction = (s, e) =>
                {
                    StateManager.GoToState(newState);
                };
                return changeStateAction;
            }

            private EventHandler PreviousHandler { get; set; }
            private EventHandler CreatePrevious()
            {
                if (PreviousHandler != null) return PreviousHandler;

                PreviousHandler = (s, e) =>
                {
                    StateManager.GoBackToPreviousState();
                };
                return PreviousHandler;
            }


            //private EventHandler BuildHandler(TState newState)
            //{
            //    var changeStateAction= Handlers.SafeValue(newState);
            //    if (changeStateAction == null)
            //    {

            //        changeStateAction = (s, e) =>
            //        {
            //            StateManager.GoToState(newState);
            //        };
            //        Handlers[newState] = changeStateAction;
            //    }
            //    return changeStateAction;
            //}

            public Action<TData> WhenChangingFromNewState(TState newState)
            {
                var changeStateAction = HandlerCache.BuildHandler(newState, Create); //BuildHandler(newState);

                return vm =>
                {
                    Unsubscribe(vm, changeStateAction);
                };
            }

            public Action<TData> WhenChangedToPreviousState()
            {
                var changeStateAction = CreatePrevious();

                return vm =>
                {
                    Subscribe(vm, changeStateAction);
                };
            }

            public Action<TData> WhenChangingFromPreviousState()
            {
                var changeStateAction = CreatePrevious();

                return vm =>
                {
                    Unsubscribe(vm, changeStateAction);
                };
            }
        }

        private class StateDefinitionValueTargetBuilder<TState, TElement> :
           StateDefinitionBuilder<TState>, IStateDefinitionValueTargetBuilder<TState, TElement>
          where TState : struct
        {
            public TElement Target { get; set; }

        }

        private class StateDefinitionValueBuilder<TState, TElement, TPropertyValue> :
            StateDefinitionBuilder<TState>, IStateDefinitionValueBuilder<TState, TElement, TPropertyValue>
           where TState : struct
        {
            public StateValue<TElement, TPropertyValue> Value { get; set; }

        }

        private class StateCompletionBuilder<TState, TCompletion> : StateDefinitionBuilder<TState>,
            IStateCompletionBuilder<TState, TCompletion>
            where TState : struct
            where TCompletion : struct
        {
            public TCompletion Completion { get; set; }
        }

        private class StateCompletionWithDataBuilder<TState, TCompletion, TData> :
            StateCompletionBuilder<TState, TCompletion>,
    IStateCompletionWithDataBuilder<TState, TCompletion, TData>
    where TState : struct
    where TCompletion : struct
        {
            public Func<TData> Data { get; set; }
        }

        private abstract class StateWithDataCompletionBaseBuilder<TState, TStateData, TCompletion, TCompletionArgs> :
    StateCompletionBuilder<TState, TCompletion>
where TState : struct
where TCompletion : struct
where TStateData : INotifyPropertyChanged
            where TCompletionArgs: CompletionEventArgs<TCompletion>
        {
            public IStateDefinitionTypedDataWrapper<TStateData> StateDataWrapper
                => State.UntypedStateDataWrapper as IStateDefinitionTypedDataWrapper<TStateData>;

            private EventHandlerCache<TState, EventHandler<TCompletionArgs>> HandlerCache { get; }
                = new EventHandlerCache<TState, EventHandler<TCompletionArgs>>();

            public Action<TStateData> WhenChangedToNewState(TState newState)
            {
                var changeStateAction = HandlerCache.BuildHandler(newState, Create);

                return WireHandlerNewState(changeStateAction);
                //    vm =>
                //{
                //    vm.Complete += changeStateAction;
                //};
            }

            protected abstract Action<TStateData> WireHandlerNewState(EventHandler<TCompletionArgs> complete);
            protected abstract Action<TStateData> UnwireHandlerNewState(EventHandler<TCompletionArgs> complete);
            protected abstract Action<TStateData> WireHandlerPreviousState(EventHandler<TCompletionArgs> complete);
            protected abstract Action<TStateData> UnwireHandlerPreviousState(EventHandler<TCompletionArgs> complete);

            protected virtual EventHandler<TCompletionArgs> Create(TState newState)
            {

                EventHandler<TCompletionArgs> changeStateAction = (s, e) =>
                {
                    if (e.Completion.Equals(Completion))
                    {
                        StateManager.GoToState(newState);
                    }
                };

                return changeStateAction;
            }

            private EventHandler<TCompletionArgs> Previous { get; set; }
            private EventHandler<TCompletionArgs> CreatePrevious()
            {
                if (Previous != null) return Previous;
                Previous = InternalCreatePrevious();
                return Previous;
            }
            protected virtual EventHandler<TCompletionArgs> InternalCreatePrevious()
            {
                EventHandler<TCompletionArgs> changeStateAction = (s, e) =>
                {
                    if (e.Completion.Equals(Completion))
                    {
                        StateManager.GoBackToPreviousState();
                    }
                };

                return changeStateAction;
            }


            public Action<TStateData> WhenChangingFromNewState(TState newState)
            {
                var changeStateAction = HandlerCache.BuildHandler(newState, Create);

                return UnwireHandlerNewState(changeStateAction);
                //    vm =>
                //{
                //    vm.Complete -= changeStateAction;
                //};
            }

            public Action<TStateData> WhenChangedToPreviousState()
            {
                var changeStateAction = CreatePrevious();

                return WireHandlerPreviousState(changeStateAction);
                //    vm =>
                //{
                //    vm.Complete += changeStateAction;
                //};
            }

            public Action<TStateData> WhenChangingFromPreviousState()
            {
                var changeStateAction = CreatePrevious();

                return UnwireHandlerPreviousState(changeStateAction);

                //    vm =>
                //{
                //    vm.Complete -= changeStateAction;
                //};
            }
        }


        private class StateWithDataCompletionBuilder<TState, TStateData, TCompletion> :
            StateWithDataCompletionBaseBuilder<TState, TStateData,TCompletion, CompletionEventArgs<TCompletion>>,
            IStateWithDataCompletionBuilder<TState, TStateData, TCompletion>
        where TState : struct
        where TCompletion : struct
        where TStateData : INotifyPropertyChanged,ICompletion<TCompletion>
        {
            //public IStateDefinitionTypedDataWrapper<TStateData> StateDataWrapper
            //    => State.UntypedStateDataWrapper as IStateDefinitionTypedDataWrapper<TStateData>;

            //private EventHandlerCache<TState, EventHandler<CompletionEventArgs<TCompletion>>> HandlerCache { get; } 
            //    = new EventHandlerCache<TState, EventHandler<CompletionEventArgs<TCompletion>>>();

            //public Action<TStateData> WhenChangedToNewState(TState newState)
            //{
            //    var changeStateAction = HandlerCache.BuildHandler(newState, Create);

            //    return vm =>
            //    {
            //        vm.Complete += changeStateAction;
            //    };
            //}

            protected override Action<TStateData> WireHandlerNewState(EventHandler<CompletionEventArgs<TCompletion>> complete)
            {
                return vm =>
                {
                    vm.Complete += complete;
                };
            }

            protected override Action<TStateData> UnwireHandlerNewState(EventHandler<CompletionEventArgs<TCompletion>> complete)
            {
                return vm =>
                {
                    vm.Complete -= complete;
                };
            }

            protected override Action<TStateData> WireHandlerPreviousState(EventHandler<CompletionEventArgs<TCompletion>> complete)
            {
                return vm =>
                {
                    vm.Complete += complete;
                };
            }

            protected override Action<TStateData> UnwireHandlerPreviousState(EventHandler<CompletionEventArgs<TCompletion>> complete)
            {
                return vm =>
                {
                    vm.Complete -= complete;
                };
            }

            //protected virtual EventHandler<CompletionEventArgs<TCompletion>> Create(TState newState)
            //{

            //    EventHandler<CompletionEventArgs<TCompletion>> changeStateAction = (s, e) =>
            //    {
            //        if (e.Completion.Equals(Completion))
            //        {
            //            StateManager.GoToState(newState);
            //        }
            //    };

            //    return changeStateAction;
            //}

            //private EventHandler<CompletionEventArgs<TCompletion>> Previous { get; set; }
            //private EventHandler<CompletionEventArgs<TCompletion>> CreatePrevious()
            //{
            //    if (Previous != null) return Previous;
            //    Previous = InternalCreatePrevious();
            //    return Previous;
            //}
            //protected virtual EventHandler<CompletionEventArgs<TCompletion>> InternalCreatePrevious()
            //{
            //    EventHandler<CompletionEventArgs<TCompletion>> changeStateAction = (s, e) =>
            //    {
            //        if (e.Completion.Equals(Completion))
            //        {
            //            StateManager.GoBackToPreviousState();
            //        }
            //    };

            //    return changeStateAction;
            //}


            //public Action<TStateData> WhenChangingFromNewState(TState newState)
            //{
            //    var changeStateAction = HandlerCache.BuildHandler(newState, Create);

            //    return vm =>
            //    {
            //        vm.Complete -= changeStateAction;
            //    };
            //}

            //public Action<TStateData> WhenChangedToPreviousState()
            //{
            //    var changeStateAction = CreatePrevious();

            //    return vm =>
            //    {
            //        vm.Complete += changeStateAction;
            //    };
            //}

            //public Action<TStateData> WhenChangingFromPreviousState()
            //{
            //    var changeStateAction = CreatePrevious();

            //    return vm =>
            //    {
            //        vm.Complete -= changeStateAction;
            //    };
            //}
        }
        
        private class StateWithDataCompletionWithDataEventBuilder<TState, TStateData, TCompletion, TData> :
            StateWithDataCompletionBaseBuilder<TState, TStateData, TCompletion, CompletionWithDataEventArgs<TCompletion,TData>>,
            IStateWithDataCompletionWithDataEventBuilder<TState, TStateData, TCompletion, TData>
            where TState : struct
            where TCompletion : struct
            where TStateData : INotifyPropertyChanged, ICompletionWithData<TCompletion,TData>
        {
            protected override Action<TStateData> WireHandlerNewState(EventHandler<CompletionWithDataEventArgs<TCompletion,TData>> complete)
            {
                return vm =>
                {
                    vm.CompleteWithData += complete;
                };
            }

            protected override Action<TStateData> UnwireHandlerNewState(EventHandler<CompletionWithDataEventArgs<TCompletion, TData>> complete)
            {
                return vm =>
                {
                    vm.CompleteWithData -= complete;
                };
            }

            protected override Action<TStateData> WireHandlerPreviousState(EventHandler<CompletionWithDataEventArgs<TCompletion, TData>> complete)
            {
                return vm =>
                {
                    vm.CompleteWithData += complete;
                };
            }

            protected override Action<TStateData> UnwireHandlerPreviousState(EventHandler<CompletionWithDataEventArgs<TCompletion, TData>> complete)
            {
                return vm =>
                {
                    vm.CompleteWithData -= complete;
                };
            }


            protected override EventHandler<CompletionWithDataEventArgs<TCompletion,TData>> Create(TState newState)
            {

                var changeStateAction = new EventHandler<CompletionWithDataEventArgs<TCompletion,TData>>((s, e) =>
                {
                    var dataVal = e.Data;
                    if (e.Completion.Equals(Completion))
                    {
                        StateManager.GoToStateWithData(newState, dataVal);
                    }
                });
                return changeStateAction;


            }

        }

        private class StateWithDataCompletionWithDataBuilder<TState, TStateData, TCompletion, TData> :
            StateWithDataCompletionBuilder<TState, TStateData, TCompletion>,
            IStateWithDataCompletionWithDataBuilder<TState, TStateData, TCompletion, TData>
            where TState : struct
            where TCompletion : struct
            where TStateData : INotifyPropertyChanged,ICompletion<TCompletion>
        {
            public Func<TStateData, TData> Data { get; set; }

            protected override EventHandler<CompletionEventArgs<TCompletion>> Create(TState newState)
            {

                var changeStateAction = new EventHandler<CompletionEventArgs<TCompletion>>((s, e) =>
                {
                    var dataVal = default(TData);
                    var data = Data;
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
                    if (e.Completion.Equals(Completion))
                    {
                        StateManager.GoToStateWithData(newState, dataVal);
                    }
                });
                return changeStateAction;
                

            }

            protected override EventHandler<CompletionEventArgs<TCompletion>> InternalCreatePrevious()
            {
                EventHandler<CompletionEventArgs<TCompletion>> changeStateAction = (s, e) =>
                {
                    if (e.Completion.Equals(Completion))
                    {
                        StateManager.GoBackToPreviousState();
                    }
                };

                return changeStateAction;
            }
        }

        #endregion


        public static IStateGroupBuilder<TState> Group<TState>
            (this IStateManager vsm) where TState : struct

        {
            if (vsm == null) return null;

            var existing = vsm.StateGroups.SafeValue(typeof(TState)) as IStateGroup<TState>;
            if (existing == null)
            {
                existing = new StateGroup<TState>();
                vsm?.AddStateGroup(existing);
            }

            return new StateGroupBuilder<TState>
            {
                StateManager = vsm,
                StateGroup = existing
            };
        }

        public static IStateGroupBuilder<TState> Group<TState>(
            this IStateBuilder vsmGroup)
            where TState : struct
        {
            if (vsmGroup == null) return null;
            return vsmGroup.StateManager.Group<TState>();
        }



        public static IStateGroupBuilder<TState> WithHistory<TState>(
    this IStateGroupBuilder<TState> vsmGroup)
    where TState : struct
        {
            if (vsmGroup == null) return null;
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
            if (vsmGroup == null) return null;
            vsmGroup.StateGroup.DefineAllStates();
            return vsmGroup;
        }


        public static
            IStateDefinitionBuilder<TState> DefineState<TState>(
            this IStateGroupBuilder<TState> vsmGroup,
            TState state)
            where TState : struct
        {
            if (vsmGroup == null) return null;
            //var vs = new StateDefinition<TState> { State = state };
            var vs = vsmGroup.StateGroup.DefineState(state);
            if (vs == null) return null;
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
            if (vsmGroup == null) return null;

            if (trigger == null) return vsmGroup;

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

        //public class StateEventBinder
        //{
        //    public Action<EventHandler> Subscribe { get; set; }
        //    public Action<EventHandler> Unsubscribe { get; set; }
        //}



        //public class StateCompletionWithDataBinder<TCompletion, TData> : StateCompletionBinder<TCompletion>
        //    where TCompletion : struct
        //{

        //    public Func<TData> CompletionData { get; set; }

        //}

        public static IStateCompletionBuilder<TState, TCompletion>
      OnComplete<TState, TCompletion>(
      this IStateDefinitionBuilder<TState> smInfo,
           TCompletion completion)
      where TState : struct
       where TCompletion : struct
        {
            if (smInfo == null) return null;
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


        public static IStateCompletionBuilder<TState, DefaultCompletion>
          OnDefaultComplete<TState>(
          this IStateDefinitionBuilder<TState> smInfo)
          where TState : struct
        {
            if (smInfo == null) return null;
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

        public static IStateWithDataCompletionBuilder<TState, TStateData, TCompletion>
            OnComplete<TState, TStateData, TCompletion>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
            TCompletion completion)
            where TState : struct
            where TCompletion : struct
            where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
        {
            if (smInfo == null) return null;
            return new StateWithDataCompletionBuilder<TState, TStateData, TCompletion>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = completion
            };
        }

        public static
           IStateWithDataCompletionBuilder<TState, TStateData, DefaultCompletion>
          OnDefaultComplete<TState, TStateData>(
          this
           IStateDefinitionWithDataBuilder<TState, TStateData> smInfo)
          where TState : struct
          where TStateData : INotifyPropertyChanged, ICompletion<DefaultCompletion>
        {
            if (smInfo == null) return null;

            return new StateWithDataCompletionBuilder<TState, TStateData, DefaultCompletion>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = DefaultCompletion.Complete
            };

            //var binder = new StateCompletionBinder<DefaultCompletion> { Completion = DefaultCompletion.Complete };
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
            if (smInfo == null) return null;
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

        public static IStateCompletionWithDataBuilder<TState, DefaultCompletion, TData>
          OnDefaultCompleteWithData<TState, TData>(
          this IStateDefinitionBuilder<TState> smInfo,
               Func<TData> completionData)
          where TState : struct
        {
            if (smInfo == null) return null;
            return new StateCompletionWithDataBuilder<TState, DefaultCompletion, TData>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = DefaultCompletion.Complete,
                Data = completionData
            };

            //if (smInfo?.Item1 == null || smInfo.Item2 == null) return null;


            //var binder = new StateCompletionWithDataBinder<DefaultCompletion, TData>
            //{
            //    Completion = DefaultCompletion.Complete,
            //    CompletionData = completionData
            //};
            //return smInfo.Extend(binder);

        }
        
        public static IStateWithDataCompletionWithDataEventBuilder<TState, TStateData, TCompletion, TData>
                 OnCompleteWithDataEvent<TState, TStateData, TCompletion, TData>(
                 this
                  IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
                      TCompletion completion)
                 where TState : struct
                 where TStateData : INotifyPropertyChanged, ICompletionWithData<TCompletion,TData>
                  where TCompletion : struct
        {
            return new StateWithDataCompletionWithDataEventBuilder<TState, TStateData, TCompletion, TData>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = completion
            };

            //var binder = new ViewModelStateCompletionWithDataBinder<TStateData, TCompletion, TData> { Completion = completion, CompletionData = completionData };
            //return smInfo.Extend(binder);

        }



        public static IStateWithDataCompletionWithDataBuilder<TState, TStateData, TCompletion, TData>
                 OnCompleteWithData<TState, TStateData, TCompletion, TData>(
                 this
                  IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
                      TCompletion completion,
                      Func<TStateData, TData> completionData)
                 where TState : struct
                 where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
                  where TCompletion : struct
        {
            return new StateWithDataCompletionWithDataBuilder<TState, TStateData, TCompletion, TData>
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


        public static IStateWithDataCompletionWithDataBuilder<TState,TStateData, DefaultCompletion, TData>
          OnDefaultCompleteWithData<TState, TStateData, TData>(
          this IStateDefinitionWithDataBuilder<TState,TStateData> smInfo,
               Func<TStateData,TData> completionData)
          where TState : struct
            where TStateData : INotifyPropertyChanged, ICompletion<DefaultCompletion>
        {
            if (smInfo == null) return null;
            return new StateWithDataCompletionWithDataBuilder<TState, TStateData, DefaultCompletion, TData>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                Completion = DefaultCompletion.Complete,
                Data = completionData
            };

            //if (smInfo?.Item1 == null || smInfo.Item2 == null) return null;


            //var binder = new StateCompletionWithDataBinder<DefaultCompletion, TData>
            //{
            //    Completion = DefaultCompletion.Complete,
            //    CompletionData = completionData
            //};
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





        public static
            IStateDefinitionValueTargetBuilder<TState, TElement> Target<TState, TElement>(
            this IStateDefinitionBuilder<TState> vsmGroup, TElement element) where TState : struct
        {
            if (vsmGroup == null || element==null) return null;
            
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
       IStateDefinitionValueBuilder<TState, TElement, TPropertyValue>
       Change<TState, TElement, TPropertyValue>(
       this IStateDefinitionValueTargetBuilder<TState, TElement> vsmGroup,
       Expression<Func< TPropertyValue>> getter,
       Action<TElement, TPropertyValue> setter = null) where TState : struct
        {
            if (vsmGroup == null || getter == null) return null;

            if (setter == null)
            {
                var propertyName = (getter.Body as MemberExpression)?.Member.Name;
                var pinfo = vsmGroup.Target.GetType().GetRuntimeProperty(propertyName);
                setter = (element, value) =>
                {
                    pinfo.SetValue(element, value);
                };
            }

            var vsv = new StateValue<TElement, TPropertyValue>
            {
                Key = new Tuple<object, string>(vsmGroup.Target, (getter.Body as MemberExpression)?.Member.Name),
                Element = vsmGroup.Target,
                Getter = (vm)=> getter.Compile().Invoke(),
                Setter = setter
            };
            return new StateDefinitionValueBuilder<TState, TElement, TPropertyValue>
            {
                StateManager = vsmGroup.StateManager,
                StateGroup = vsmGroup.StateGroup,
                State = vsmGroup.State,
                Value = vsv
            };
            //return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinition<TState>, StateValue<TElement, TPropertyValue>>(
            //    vsmGroup.Item1, vsmGroup.Item2, vsmGroup.Item3, vsv);
        }


        public static
            IStateDefinitionValueBuilder<TState, TElement, TPropertyValue>
            Change<TState, TElement, TPropertyValue>(
            this IStateDefinitionValueTargetBuilder<TState, TElement> vsmGroup,
            Expression<Func<TElement, TPropertyValue>> getter,
            Action<TElement, TPropertyValue> setter=null) where TState : struct
        {
            if (vsmGroup == null || getter==null) return null;

            if (setter == null)
            {
                var propertyName = (getter.Body as MemberExpression)?.Member.Name;
                var pinfo = vsmGroup.Target.GetType().GetRuntimeProperty(propertyName);
                setter = (element, value) =>
                {
                    pinfo.SetValue(element, value);
                };
            }

            var vsv = new StateValue<TElement, TPropertyValue>
            {
                Key = new Tuple<object, string>(vsmGroup.Target, (getter.Body as MemberExpression)?.Member.Name),
                Element = vsmGroup.Target,
                Getter = getter.Compile(),
                Setter = setter
            };
            return new StateDefinitionValueBuilder<TState, TElement, TPropertyValue>
            {
                StateManager = vsmGroup.StateManager,
                StateGroup = vsmGroup.StateGroup,
                State = vsmGroup.State,
                Value = vsv
            };
            //return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinition<TState>, StateValue<TElement, TPropertyValue>>(
            //    vsmGroup.Item1, vsmGroup.Item2, vsmGroup.Item3, vsv);
        }

        

        public static
            IStateDefinitionBuilder<TState>
            ToValue<TState, TElement, TPropertyValue>(
            this
            IStateDefinitionValueBuilder<TState, TElement, TPropertyValue> vsmGroup,
            TPropertyValue value) where TState : struct
        {
            if (vsmGroup == null) return null;
            if (value == null) return vsmGroup;

            vsmGroup.Value.Value = value;
            vsmGroup.State.Values.Add(vsmGroup.Value);
            //vsmGroup.Item4.Value = value;
            //vsmGroup.Item3.Values.Add(vsmGroup.Item4);
            return vsmGroup;
        }

        public static
          IStateDefinitionBuilder<TState> ChangePropertyValue<TState, TPropertyValue>(
          this IStateDefinitionBuilder<TState> vsmGroup,
          Expression<Func<TPropertyValue>> getter,
          TPropertyValue value) where TState : struct
        {
            var property = (getter.Body as MemberExpression)?.Expression as ConstantExpression;
            var element = property?.Value;

            return vsmGroup
                .Target(element)
                .Change(getter)
                .ToValue(value);
        }


        public static
            IStateDefinitionBuilder<TState> ChangePropertyValue<TState, TElement, TPropertyValue>(
            this IStateDefinitionBuilder<TState> vsmGroup, 
            TElement element,
            Expression<Func<TElement, TPropertyValue>> getter,
            TPropertyValue value) where TState : struct
        {
            return vsmGroup
                .Target(element)
                .Change(getter)
                .ToValue(value);
        }
        

        public static
            IStateDefinitionBuilder<TState> ChangePropertyValue<TState, TPropertyValue>(
            this IStateDefinitionBuilder<TState> vsmGroup,
            Expression<Func<object,TPropertyValue>> getter,
            TPropertyValue value) where TState : struct
        {
            var property = (getter.Body as MemberExpression)?.Expression as ConstantExpression;
            var element = property?.Value;

            return vsmGroup
                .Target(element)
                .Change(getter)
                .ToValue(value);
        }

        //public static StateValue<TElement, TPropertyValue> ChangeProperty<TElement, TPropertyValue>(
        //    this TElement element, Expression<Func<TElement, TPropertyValue>> getter, Action<TElement, TPropertyValue> setter)

        //{
        //    return new StateValue<TElement, TPropertyValue>
        //    {
        //        Key = new Tuple<object, string>(element, (getter.Body as MemberExpression)?.Member.Name),
        //        Element = element,
        //        Getter = getter.Compile(),
        //        Setter = setter
        //    };
        //}

        //public static StateValue<TElement, TPropertyValue> ToValue<TElement, TPropertyValue>(
        //    this StateValue<TElement, TPropertyValue> state, TPropertyValue newValue)

        //{
        //    state.Value = newValue;
        //    return state;
        //}

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
            this IStateDefinitionBuilder<TState> smInfo,
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
            IStateDefinitionWithDataBuilder<TState, TStateData>
           DefineStateWithData<TState, TStateData>(
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

            IStateDefinitionWithDataBuilder<TState, TStateData>
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

        public static IStateDefinitionWithDataBuilder<TState, TStateData> WhenAboutToChange<TState, TStateData>(
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


        public static IStateDefinitionWithDataBuilder<TState, TStateData> 
            InitializeNewState<TState, TStateData, TNewStateData,TData>(
    this IStateDefinitionWithDataChangeStateWithDataBuilder<TState, TStateData,TData> smInfo,
    Action<TNewStateData, TData> action) 
            where TState : struct
            where TStateData : INotifyPropertyChanged
            where TNewStateData : INotifyPropertyChanged
        {
#pragma warning disable 1998  // Convert sync method into async call
            return smInfo.InitializeNewState<TState, TStateData, TNewStateData, TData>(async (vm, s) => action(vm, s));
#pragma warning restore 1998
        }

        public static IStateDefinitionWithDataBuilder<TState, TStateData>
            InitializeNewState<TState, TStateData, TNewStateData, TData>(
            this IStateDefinitionWithDataChangeStateWithDataBuilder<TState, TStateData, TData> existingInfo,
            Func<TNewStateData, TData, Task> action) 
            where TState : struct
            where TStateData : INotifyPropertyChanged
            where TNewStateData : INotifyPropertyChanged
        {
            var smInfo = existingInfo.DefineStateWithData<TState, TNewStateData>(existingInfo.NewState);
        
            if (smInfo?.StateDataWrapper == null) return null;
            var stateDefinition = smInfo.StateDataWrapper;

            "Adding Behaviour: ChangedToWithDataViewModel".Log();

            var modAction = new Func<TNewStateData, string, Task>((vm, d) =>
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
            return existingInfo;
        }



        public static IStateDefinitionWithDataBuilder<TState, TStateData> WhenChangedTo<TState, TStateData>(
            this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo,
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

        public static IStateDefinitionWithDataEventBuilder<TState, TStateData>

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



        //public static IStateDefinitionWithDataBuilder<TState, TStateData> ChangeState<TState, TStateData>(
        //    this IStateDefinitionWithDataEventBuilder<TState, TStateData> smInfo,
        //    TState stateToChangeTo) where TState : struct
        //    where TStateData : INotifyPropertyChanged
        //{
        //    if (smInfo?.State == null) return null;
        //    //if (smInfo?.Item1 == null ||
        //    //    smInfo.Item2 == null ||
        //    //    smInfo.Item3 == null) return null;

        //    var binder = smInfo;
        //    var sm = smInfo.StateManager;
        //    var newState = stateToChangeTo;
        //    var changeStateAction = new EventHandler((s, e) =>
        //    {
        //        sm.GoToState(newState);
        //    });

        //    var smreturn = smInfo;

        //    var returnd = smreturn.WhenChangedTo(vm =>
        //    {
        //        binder.Subscribe(vm, changeStateAction);
        //    })
        //        .WhenChangingFrom(vm =>
        //        {
        //            binder.Unsubscribe(vm, changeStateAction);
        //        });

        //    return returnd;

        //    //new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        //}

        public static IStateDefinitionWithDataChangeStateWithDataBuilder<TState, TStateData,TData> 
            ChangeState<TState, TStateData, TData>(
            this IStateWithDataActionData<TState, TStateData,TData> smInfo,
            TState stateToChangeTo) 
            where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            if (smInfo == null) return null;

            
            var returnd = smInfo
                .WhenChangedTo(smInfo.WhenChangedToNewState(stateToChangeTo))
                .WhenChangingFrom(smInfo.WhenChangingFromNewState(stateToChangeTo))
                .IncludeStateInit<TState,TStateData,TData>(stateToChangeTo);

            return returnd;

            //new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }

        private static IStateDefinitionWithDataChangeStateWithDataBuilder<TState, TStateData, TNewStateData>
            IncludeStateInit
            <TState, TStateData, TNewStateData>(this IStateDefinitionWithDataBuilder<TState, TStateData> smInfo, TState newState)
            where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            return new StateDefinitionWithDataChangeStateWithDataBuilder<TState, TStateData, TNewStateData>
            {
                StateManager = smInfo.StateManager,
                StateGroup = smInfo.StateGroup,
                State = smInfo.State,
                NewState = newState
            };
        }

        public static IStateDefinitionWithDataBuilder<TState, TStateData> ChangeState<TState, TStateData>(
    this IStateWithDataAction<TState, TStateData> smInfo,
    TState stateToChangeTo) where TState : struct
    where TStateData : INotifyPropertyChanged
        {
            if (smInfo == null) return null;


            var returnd = smInfo
                .WhenChangedTo(smInfo.WhenChangedToNewState(stateToChangeTo))
                .WhenChangingFrom(smInfo.WhenChangingFromNewState(stateToChangeTo));

            return returnd;

            //new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }


        public static IStateDefinitionWithDataBuilder<TState, TStateData> ChangeToPreviousState<TState, TStateData>(
            this IStateWithDataAction<TState, TStateData> smInfo) where TState : struct
            where TStateData : INotifyPropertyChanged
        {
            if (smInfo == null) return null;


            var returnd = smInfo
                .WhenChangedTo(smInfo.WhenChangedToPreviousState())
                .WhenChangingFrom(smInfo.WhenChangingFromPreviousState());

            return returnd;

            //new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        }


        //public static IStateDefinitionWithDataBuilder<TState, TStateData>
        //    ChangeToPreviousState<TState, TStateData>(
        //    this IStateDefinitionWithDataEventBuilder<TState, TStateData> smInfo)
        //    where TState : struct
        //    where TStateData : INotifyPropertyChanged
        //{
        //    if (smInfo.State == null) return null;
        //    //if (smInfo?.Item1 == null ||
        //    //    smInfo.Item2 == null ||
        //    //    smInfo.Item3 == null) return null;

        //    var binder = smInfo;
        //    var sm = smInfo.StateManager;
        //    var changeStateAction = new EventHandler((s, e) =>
        //    {
        //        sm.GoBackToPreviousState();
        //    });

        //    var smreturn = smInfo;

        //    var returnd = smreturn.WhenChangedTo(vm =>
        //    {
        //        binder.Subscribe(vm, changeStateAction);
        //    })
        //        .WhenChangingFrom(vm =>
        //        {
        //            binder.Unsubscribe(vm, changeStateAction);
        //        });
        //    return returnd;
        //    //            return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        //}

        //public static
        //    IStateDefinitionWithDataBuilder<TState, TStateData>
        //    ChangeState<TState, TStateData, TCompletion>(
        //    this IStateWithDataCompletionBuilder<TState, TStateData, TCompletion> smInfo,
        //    TState stateToChangeTo)
        //    where TState : struct
        //    where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
        //    where TCompletion : struct
        //{
        //    if (smInfo?.State == null) return null;
        //    //if (smInfo?.Item1 == null ||
        //    //    smInfo.Item2 == null ||
        //    //    smInfo.Item3 == null) return null;

        //    var comp = smInfo?.Completion;
        //    var sm = smInfo?.StateManager;
        //    var newState = stateToChangeTo;
        //    var changeStateAction = new EventHandler<CompletionEventArgs<TCompletion>>((s, e) =>
        //    {
        //        if (e.Completion.Equals(comp))
        //        {
        //            sm.GoToState(newState);
        //        }
        //    });

        //    var smreturn = smInfo;

        //    var returnd = smreturn.WhenChangedTo(vm =>
        //    {
        //        vm.Complete += changeStateAction;
        //    })
        //        .WhenChangingFrom(vm =>
        //        {
        //            vm.Complete -= changeStateAction;
        //        });
        //    return smreturn;
        //    //            return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        //}


        //public static
        //    IStateDefinitionWithDataBuilder<TState, TStateData>
        //    ChangeState<TState, TStateData, TCompletion, TData>(
        //    this IStateWithDataCompletionWithDataBuilder<TState, TStateData, TCompletion, TData> smInfo,
        //    TState stateToChangeTo)
        //    where TState : struct
        //    where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
        //    where TCompletion : struct
        //{
        //    //if (smInfo?.Item1 == null ||
        //    //    smInfo.Item2 == null ||
        //    //    smInfo.Item3 == null) return null;

        //    var comp = smInfo?.Completion;
        //    var data = smInfo?.Data;
        //    var sm = smInfo.StateManager;
        //    var newState = stateToChangeTo;
        //    var changeStateAction = new EventHandler<CompletionEventArgs<TCompletion>>((s, e) =>
        //    {
        //        var dataVal = default(TData);
        //        var cc = e as CompletionWithDataEventArgs<TCompletion, TData>;
        //        if (cc != null)
        //        {
        //            dataVal = cc.Data;
        //        }
        //        if (data != null)
        //        {
        //            var vvm = (TStateData)s;
        //            dataVal = data(vvm);
        //        }
        //        if (e.Completion.Equals(comp))
        //        {
        //            sm.GoToStateWithData(newState, dataVal);
        //        }
        //    });



        //    var smreturn = smInfo;

        //    smreturn.WhenChangedTo(vm =>
        //    {
        //        vm.Complete += changeStateAction;
        //    })
        //        .WhenChangingFrom(vm =>
        //        {
        //            vm.Complete -= changeStateAction;
        //        });
        //    return smreturn;
        //    //            return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        //}
        //public static
        //    IStateDefinitionWithDataBuilder<TState, TStateData>
        //    ChangeToPreviousState<TState, TStateData, TCompletion>(
        //    this
        //    IStateWithDataCompletionBuilder<TState, TStateData, TCompletion> smInfo
        //    )
        //    where TState : struct
        //    where TStateData : INotifyPropertyChanged, ICompletion<TCompletion>
        //    where TCompletion : struct
        //{
        //    //if (smInfo?.Item1 == null ||
        //    //    smInfo.Item2 == null ||
        //    //    smInfo.Item3 == null) return null;

        //    var comp = smInfo?.Completion;
        //    var sm = smInfo.StateManager;
        //    var changeStateAction = new EventHandler<CompletionEventArgs<TCompletion>>((s, e) =>
        //    {
        //        if (e.Completion.Equals(comp))
        //        {
        //            sm.GoBackToPreviousState();
        //        }
        //    });



        //    var smreturn = smInfo;

        //    var returnd = smreturn.WhenChangedTo(vm =>
        //    {
        //        vm.Complete += changeStateAction;
        //    })
        //        .WhenChangingFrom(vm =>
        //        {
        //            vm.Complete -= changeStateAction;
        //        });
        //    return returnd;
        //    //            return new Tuple<IStateManager, IStateGroup<TState>, IStateDefinitionWithData<TState, TStateData>>(smInfo.Item1, smInfo.Item2, smInfo.Item3);
        //}



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

   


}