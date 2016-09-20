using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BuildIt.States
{
    public interface IHasStateData
    {
        INotifyPropertyChanged CurrentStateData { get; }
    }

    public interface IDependencyContainer
    {
        IDisposable StartUpdate();
        void EndUpdate();
        void Register<TTypeToRegister, TInterfaceTypeToRegisterAs>();

        void Register<TTypeToRegister>();

        void RegisterType(Type typeToRegister);
    }

    public interface IRegisterDependencies
    {
        void RegisterDependencies(IDependencyContainer container);
    }


    public interface IAboutToLeave
    {
        Task AboutToLeave(CancelEventArgs cancel);
    }

    public interface ILeaving
    {
        Task Leaving();
    }

    public interface IArriving
    {
        Task Arriving();
    }

    public class StateGroup<TState> :  IStateGroup<TState>, IHasStateData, IRegisterDependencies
        where TState : struct
        
    {
        public event EventHandler GoToPreviousStateIsBlockedChanged;

        public event EventHandler<StateEventArgs<TState>> StateChanged;

        public TState CurrentState { get; private set; }

        public IDependencyContainer DependencyContainer { get; set; }

        public IUIExecutionContext UIContext { get; set; }

        public IDictionary<TState, IStateDefinition<TState>> States { get; } =
            new Dictionary<TState, IStateDefinition<TState>>();

        private IDictionary<Tuple<object, string>, IDefaultValue> DefaultValues { get; }
            = new Dictionary<Tuple<object, string>, IDefaultValue>();


        private IDictionary<Type, INotifyPropertyChanged> StateDataCache { get; } =
           new Dictionary<Type, INotifyPropertyChanged>();

        public INotifyPropertyChanged CurrentStateData { get; set; }

        public INotifyPropertyChanged Existing(Type stateDataType)
        {
            if (stateDataType == null) return null;

            INotifyPropertyChanged existing;
            StateDataCache.TryGetValue(stateDataType, out existing);
            return existing;
        }


        private IList<IStateTrigger> Triggers { get; }=new List<IStateTrigger>();

        public void WatchTrigger(IStateTrigger trigger)
        {
            trigger.IsActiveChanged += Trigger_IsActiveChanged;
            Triggers.Add(trigger);
        }

        private void Trigger_IsActiveChanged(object sender, EventArgs e)
        {
            UpdateStatesByTriggers();
        }

        private async void UpdateStatesByTriggers()
        {
            var current = States.SafeValue(CurrentState);
            if (current != null)
            {
                // Don't change state if current state triggers are still active
                if (StateTriggersActive(current)) return;
            }

            var firstActiveState = States.FirstOrDefault(x => StateTriggersActive(x.Value));
            // If there's no active state, then just return
            if (firstActiveState.Key.Equals(default(TState))) return;

            if (CurrentState.Equals(firstActiveState.Key)) return;

            await ChangeTo(firstActiveState.Key);

        }

        private bool StateTriggersActive(IStateDefinition<TState> state)
        {
            return state.Triggers.All(x => x.IsActive);
        }

        public bool TrackHistory { get; set; } = false;

        public bool HasHistory
        {
            get
            {
                if(TrackHistory==false)throw new Exception("History tracking not enabled");
                return History.Count > 0;
            }
        }

        public virtual bool GoToPreviousStateIsBlocked
        {
            get
            {
                // ReSharper disable once SuspiciousTypeConversion.Global 
                var isBlockable = CurrentStateData as IIsAbleToBeBlocked;
                return isBlockable?.IsBlocked ?? false;
            }
        }

        private void IsBlockable_IsBlockedChanged(object sender, EventArgs e)
        {
            OnGoToPreviousStateIsBlockedChanged();
        }


        protected void OnGoToPreviousStateIsBlockedChanged()
        {
            GoToPreviousStateIsBlockedChanged.SafeRaise(this);
        }


        private Stack<TState> History { get; } = new Stack<TState>(); 

        private IStateDefinition<TState> State<TFindState>(TFindState state)
            where TFindState : struct
        {
            if (typeof(TFindState) != typeof(TState)) return null;
            var searchState = (TState)(object)state;
            return States.SafeValue(searchState);
        }

        //public void TransitionTo<TFindState>(TFindState state) where TFindState : struct
        //{
        //    var visualState = State(state);
        //    visualState?.TransitionTo(DefaultValues);
        //}

        public void DefineAllStates()
        {
            var vals = Enum.GetValues(typeof(TState));
            foreach (var enumVal in vals)
            {
                $"Defining state {enumVal}".Log();
                DefineState((TState)enumVal);
            }
        }

        public virtual IStateDefinition<TState> DefineState(IStateDefinition<TState> stateDefinition)
        {
            if (stateDefinition == null)
            {
                $"Can't define null state definition".Log();
                return null;
            }

            var existing = States.SafeValue(stateDefinition.State);
            if (existing != null)
            {
                $"State definition already defined, returning existing instance - {existing.GetType().Name}".Log();
                return existing;
            }

            $"Defining state of type {stateDefinition.GetType().Name}".Log();
            States[stateDefinition.State] = stateDefinition;
            return stateDefinition;
        }

        public virtual IStateDefinition<TState> DefineState(TState state)
        {
            // Don't ever add the default value (eg Base) state
            if (default(TState).Equals(state))
            {
                $"Attempted to add the default state definition".Log();
                return null;
            }
            var stateDefinition = new StateDefinition<TState> { State = state };
            return DefineState(stateDefinition);
        }


        public virtual IStateDefinitionWithData<TState, TStateData> DefineStateWithData<TStateData>(TState state) 
            where TStateData : INotifyPropertyChanged
        {
            // Don't ever add the default value (eg Base) state
            if (default(TState).Equals(state))
            {
                $"Attempted to add the default state definition".Log();
                return null;
            }


            $"Defining state for {typeof(TState).Name} with data type {typeof(TStateData)}".Log();
            var stateDefinition = new StateDefinition<TState>
            {
                State = state,
                UntypedStateDataWrapper = new StateDefinitionTypedDataWrapper<TStateData>()
            };
            var stateDef = DefineState(stateDefinition);
            return new StateDefinitionWithDataWrapper<TState, TStateData> {State = stateDef};
        }


        public async Task<bool> ChangeTo<TFindState>(TFindState findState, bool useTransitions = true)
            where TFindState : struct
        {
            if (typeof (TFindState) != typeof (TState))
            {
                $"Attempting to change to the wrong state type {typeof (TFindState)}".Log();
                return false;
            }

            var newState = (TState) (object) findState;

            return await ChangeTo(newState, useTransitions);
        }

        public async Task<bool> ChangeToWithData<TFindState,TData>(TFindState findState, TData data, bool useTransitions = true)
           where TFindState : struct
        {
            if (typeof(TFindState) != typeof(TState))
            {
                $"Attempting to change to the wrong state type {typeof(TFindState)}".Log();
                return false;
            }

            var newState = (TState)(object)findState;

            var dataAsString = data.EncodeJson();
            return await ChangeTo(newState, true, useTransitions, dataAsString);
        }

        public async Task<bool> ChangeBackTo<TFindState>(TFindState findState, bool useTransitions = true)
            where TFindState : struct
        {
            if(TrackHistory==false)throw new Exception("History tracking not enabled");

            if (typeof(TFindState) != typeof(TState))
            {
                $"Attempting to change to the wrong state type {typeof(TFindState)}".Log();
                return false;
            }

            var newState = (TState)(object)findState;

            return await ChangeBackTo(newState, useTransitions);
        }

        public async Task<bool> ChangeToPrevious(bool useTransitions = true)
        {
            if (TrackHistory == false) throw new Exception("History tracking not enabled");

            if (History.Count == 0) return false;

            var previous = History.Peek();

            return await ChangeBackTo(previous, useTransitions);
        }

        public IStateBinder Bind(IStateGroup groupToBindTo, bool bothDirections =true)
        {
            var sg = groupToBindTo as IStateGroup<TState>; // This includes INotifyStateChanged
            if (sg == null) return null;

            return new StateGroupBinder<TState>(this,sg, bothDirections);
        }

        private class StateGroupBinder<TStateBind> : IStateBinder
            where TStateBind : struct
        {
            private IStateGroup StateGroup { get; }
            private IStateGroup<TStateBind> Source { get; }

            private bool BothDirections { get; }
            public StateGroupBinder(IStateGroup sg, IStateGroup<TStateBind> source, bool bothDirections)
            {
                BothDirections = bothDirections;
                StateGroup = sg;
                Source = source;

                Source.StateChanged += Source_StateChanged;
                if (bothDirections)
                {
                    var dest = sg as IStateGroup<TStateBind>;
                    if (dest == null) return;

                    dest.StateChanged += Dest_StateChanged;
                }
            }

            private void Source_StateChanged(object sender, StateEventArgs<TStateBind> e)
            {
                if (e.IsNewState)
                {
                    StateGroup.ChangeTo(e.State, e.UseTransitions);
                }
                else
                {
                    StateGroup.ChangeBackTo(e.State, e.UseTransitions);
                }
            }

            private void Dest_StateChanged(object sender, StateEventArgs<TStateBind> e)
            {
                if (e.IsNewState)
                {
                    Source.ChangeTo(e.State, e.UseTransitions);
                }
                else
                {
                    Source.ChangeBackTo(e.State, e.UseTransitions);
                }
            }

            public void Unbind()
            {
                Source.StateChanged -= Source_StateChanged;

                if (BothDirections)
                {
                    var dest = StateGroup as IStateGroup<TStateBind>;
                    if (dest == null) return;

                    dest.StateChanged += Dest_StateChanged;
                }
            }
        }

        public async Task<bool> ChangeTo(TState newState, bool useTransitions = true)
        {
            return await ChangeTo(newState, true, useTransitions);
        }


        public async Task<bool> ChangeBackTo(TState newState, bool useTransitions = true)
        {
            return await ChangeTo(newState, false, useTransitions);
        }

        private async Task<bool> ChangeTo(TState newState, bool isNewState, bool useTransitions, string data=null)
        {
            
    $"Changing to state {newState} ({useTransitions})".Log();
            var current = CurrentState;
            if (current.Equals(newState))
            {
                "Transitioning to same state - doing nothing".Log();
                return true;
            }


            if (!current.Equals(default(TState)))
            {
                $"Current state is {current}".Log();
                var currentStateDef = States[current];
                var cancel = new CancelEventArgs();
                if (currentStateDef.AboutToChangeFrom != null)
                {
                    "Invoking 'AboutToChangeFrom' on current state definition".Log();
                    await currentStateDef.AboutToChangeFrom(cancel);
                    "'AboutToChangeFrom' completed".Log();
                }
                if (cancel.Cancel)
                {
                    "Cancelling state transition invoking 'AboutToChangeFrom'".Log();
                    return false;
                }
            }


            try
            {
                "Invoking internal ChangeToState to perform state change".Log();
                var proceed = await ChangeToState(current, newState, isNewState);
                if (!proceed)
                {
                    "Unable to complete ChangeToState so exiting the ChangeTo, returning false".Log();
                    return false;
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
                return false;
            }

            $"About to updated CurrentState (currently: {CurrentState})".Log();

            if (isNewState)
            {
                var oldState = CurrentState;
                CurrentState = newState;
                if (TrackHistory && !oldState.Equals(default(TState)))
                {
                    History.Push(oldState);
                }
            }
            else
            {
                while (History.Count > 0)
                {
                    var historyState = History.Pop();
                    if (!historyState.Equals(newState)) continue;
                    CurrentState = newState;
                    break;
                }
                if (!CurrentState.Equals(newState))
                {
                    $"Unable to go back to {newState} as it doesn't exist in the state History".Log();
                    return false;
                }
            }
            $"CurrentState updated (now: {CurrentState})".Log();

            States[CurrentState].TransitionTo(DefaultValues);

            await NotifyStateChanged(newState, useTransitions, isNewState);
            
            await ChangedToState(newState,data);



            "ChangeTo completed".Log();
            return true;
        }

#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected virtual async Task NotifyStateChanged(TState newState, bool useTransitions, bool isNewState)
#pragma warning restore 1998
        {
            try
            {
                if (StateChanged != null)
                {
                    "Invoking StateChanged event (before UI context check)".Log();
                    await UIContext.RunAsync(() =>
                    {
                        "Raising StateChanged event".Log();
                        StateChanged.Invoke(this, new StateEventArgs<TState>(CurrentState, useTransitions, isNewState));
                        "Raising StateChanged event completed".Log();
                    });
                    "StateChanged event completed (after UI context check)".Log();
                }
                else
                {
                    "Nothing listening to StateChanged".Log();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
                // Ignore any errors caused by the event being raised, as 
                // the state change has still occurred
            }

        }

#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected virtual async Task<bool> ChangeToState(TState oldState, TState newState, bool isNewState)
#pragma warning restore 1998
        {
            // ReSharper disable once SuspiciousTypeConversion.Global - NOT HELPFUL
            var aboutVM = CurrentStateData as IAboutToLeave;
            var cancel = new CancelEventArgs();
            if (aboutVM != null)
            {
                "Invoking AboutToLeave".Log();
                await aboutVM.AboutToLeave(cancel);
                if (cancel.Cancel)
                {
                    "ChangeToState cancelled by AboutToLeave".Log();
                    return false;
                }
            }

            "Retrieving current state definition".Log();
            // ReSharper disable once SuspiciousTypeConversion.Global - NOT HELPFUL
            var currentVMStates = !oldState.Equals(default(TState)) ? States[oldState].UntypedStateDataWrapper : null;
            if (currentVMStates != null)
            {
                "Invoking AboutToChangeFrom for existing state definition".Log();
                await currentVMStates.InvokeAboutToChangeFrom(CurrentStateData, cancel);
                if (cancel.Cancel)
                {
                    "ChangeToState cancelled by existing state definition".Log();
                    return false;
                }
            }

            // ReSharper disable once SuspiciousTypeConversion.Global // NOT HELPFUL
            var leaving = CurrentStateData as ILeaving;
            if (leaving != null)
            {
                "Invoking Leaving on current view model".Log();
                await leaving.Leaving();
            }

            // ReSharper disable once SuspiciousTypeConversion.Global // NOT HELPFUL
            var isBlockable = CurrentStateData as IIsAbleToBeBlocked;
            if (isBlockable != null)
            {
                "Detaching event handlers for isblocked on current view model".Log();
                isBlockable.IsBlockedChanged -= IsBlockable_IsBlockedChanged;
            }


            if (currentVMStates != null)
            {
                "Invoking ChangingFrom on current state definition".Log();
                await currentVMStates.InvokeChangingFrom(CurrentStateData);
            }

            if (!oldState.Equals(default(TState)))
            {
                var currentStateDef = States[oldState];
                if (currentStateDef.ChangingFrom != null)
                {
                    "Invoking 'ChangingFrom'".Log();
                    await currentStateDef.ChangingFrom();
                }
            }

            CurrentStateData = null;
            INotifyPropertyChanged vm = null;
            currentVMStates = null; // Make sure we don't accidentally refernce the wrapper for the old state
            if (!newState.Equals(default(TState)))
            {
                var current = States[newState];// as IGenerateViewModel;
                currentVMStates = current?.UntypedStateDataWrapper;

                "Retrieving existing ViewModel for new state".Log();
                vm = Existing(currentVMStates?.StateDataType);
            }

            if (currentVMStates != null)
            {

                if (vm == null)
                {
                    //var newGen = States[newState] as IGenerateViewModel;
                    //if (newGen == null) return false;
                    "Generating ViewModel for new state".Log();
                    vm = currentVMStates.Generate();

                    "Registering dependencies".Log();
                    (vm as IRegisterDependencies)?.RegisterDependencies(DependencyContainer);

                    await currentVMStates.InvokeInitialise(vm);

                }
                var requireUI = vm as IRegisterForUIAccess;
                requireUI?.RegisterForUIAccess(this);

                StateDataCache[vm.GetType()] = vm;
                CurrentStateData = vm;
                isBlockable = CurrentStateData as IIsAbleToBeBlocked;
                if (isBlockable != null)
                {
                    isBlockable.IsBlockedChanged += IsBlockable_IsBlockedChanged;
                }
            }


            return true;
        }


#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected virtual async Task ChangedToState(TState newState, string dataAsJson)
#pragma warning restore 1998
        {

            var newStateDef = States.SafeValue<TState, IStateDefinition<TState>, IStateDefinition<TState>>(newState);
            if (newStateDef?.ChangedTo != null)
            {
                $"State definition found, of type {newStateDef.GetType().Name}, invoking ChangedTo method".Log();
                await newStateDef.ChangedTo();
                "ChangedTo completed".Log();
            }
            else
            {
                "No new state definition".Log();
            }

            if (newStateDef?.ChangedToWithJsonData != null)
            {
                $"State definition found, of type {newStateDef.GetType().Name}, invoking ChangedToWithJsonData method".Log();
                await newStateDef.ChangedToWithJsonData(dataAsJson);
                "ChangedToWithJsonData completed".Log();
            }
            else
            {
                "No new state definition".Log();
            }


            // ReSharper disable once SuspiciousTypeConversion.Global // NOT HELPFUL
            var arrived = CurrentStateData as IArriving;
            if (arrived != null)
            {
                "Invoking Arriving on new ViewModel".Log();
                await arrived.Arriving();
            }

            var currentVMStates = States[CurrentState]?.UntypedStateDataWrapper;
            if (currentVMStates != null)
            {
                "Invoking ChangedTo on new state definition".Log();
                await currentVMStates.InvokeChangedTo(CurrentStateData);
                await currentVMStates.InvokeChangedToWithData(CurrentStateData, dataAsJson);
            }

        }




        //public virtual ITransitionDefinition<TState> DefineTransition(TTransition transition)
        //{
        //    var transitionDefinition = new TransitionDefinition<TState>();
        //    Transitions[transition] = transitionDefinition;
        //    $"Defining transition of type {transition.GetType().Name}".Log();
        //    return transitionDefinition;
        //}

        //protected virtual ITransitionDefinition<TState> CreateDefaultTransition()
        //{
        //    return new TransitionDefinition<TState>();
        //}

        //public async Task<bool> Transition(TState newState, bool useTransition = true)
        //{
        //    var transition = CreateDefaultTransition();
        //    transition.EndState = newState;
        //    return await InternalTransition(transition, useTransition);
        //}

        //public async Task<bool> Transition(TTransition transitionDef, bool useTransition = true)
        //{
        //    var transition = Transitions[transitionDef];
        //    return await InternalTransition(transition, useTransition);
        //}

        //private async Task<bool> InternalTransition(ITransitionDefinition<TState> transition, bool useTransition)
        //{
        //    $"Checking initial state {CurrentState} and transition start state {transition.StartState}".Log();
        //    if (!transition.StartState.Equals(CurrentState) && !transition.StartState.Equals(default(TState)))
        //    {
        //        "Current state doesn't match start state of transition".Log();
        //        return false;
        //    }
        //    var cancel = new CancelEventArgs();
        //    "Invoking LeavingState".Log();
        //    await LeavingState(transition, CurrentState, cancel);
        //    "LeavingState completed".Log();

        //    if (cancel.Cancel)
        //    {
        //        "Transition cancelled by LeavingState".Log();
        //        return false;
        //    }

        //    "Invoking ArrivingState".Log();
        //    await ArrivingState(transition);
        //    "ArrivingState completed, now invoking ChangeTo".Log();

        //    if (!await ChangeTo(transition.EndState, useTransition))
        //    {
        //        "ChangeTo not completed, transition aborted".Log();
        //        return false;
        //    }

        //    "Invokign ArrivedState".Log();
        //    await ArrivedState(transition, CurrentState);
        //    "ArrivedState completed".Log();
        //    return true;
        //}

        //protected virtual async Task ArrivedState(ITransitionDefinition<TState> transition, TState currentState)
        //{
        //    if (transition.ArrivedState != null)
        //    {
        //        await transition.ArrivedState(currentState);
        //    }

        //    // TODO: Consider doing this for when there is data
        //    //var trans = transition as ViewModelTransitionDefinition<TState>;
        //    //if (trans?.ArrivedStateViewModel != null)
        //    //{
        //    //    await trans.ArrivedStateViewModel(currentState, CurrentViewModel);
        //    //}

        //}

        //protected virtual async Task LeavingState(ITransitionDefinition<TState> transition, TState currentState, CancelEventArgs cancel)
        //{
        //    if (transition.LeavingState != null)
        //    {
        //        await transition.LeavingState(currentState, cancel);
        //    }

        //    // TODO: Consider doing this for when there is data
        //    //if (cancel.Cancel) return;
        //    //var trans = transition as ViewModelTransitionDefinition<TState>;
        //    //if (trans?.LeavingStateViewModel != null)
        //    //{
        //    //    await trans.LeavingStateViewModel(currentState, CurrentViewModel, cancel);
        //    //}
        //}

        //protected virtual async Task ArrivingState(ITransitionDefinition<TState> transition)
        //{
        //    if (transition.ArrivingState != null)
        //    {
        //        await transition.ArrivingState(transition.EndState);
        //    }
        //}


        public void RegisterDependencies(IDependencyContainer container)
        {
            DependencyContainer = container;
            using (container.StartUpdate())
            {
                foreach (var state in States.Values.Select(x => x.UntypedStateDataWrapper).Where(x => x != null))
                {
                    container.RegisterType(state.StateDataType);
                }
            }
        }

    }
}