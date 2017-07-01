using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using BuildIt.ServiceLocation;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    public class StateGroup : IStateGroup, IHasStateData, IRegisterDependencies
    {
        public event EventHandler GoToPreviousStateIsBlockedChanged;

        public event EventHandler<StateEventArgs> StateChanged;
        public event EventHandler<StateCancelEventArgs> StateChanging;

        public virtual string CurrentStateName { get; protected set; }

        public IDependencyContainer DependencyContainer { get; set; }

        public IUIExecutionContext UIContext { get; set; }

        public IDictionary<string, IStateDefinition> States { get; } =
            new Dictionary<string, IStateDefinition>();

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


        private IList<IStateTrigger> Triggers { get; } = new List<IStateTrigger>();

        public void WatchTrigger(IStateTrigger trigger)
        {
            trigger.IsActiveChanged += Trigger_IsActiveChanged;
            Triggers.Add(trigger);
        }

        private void Trigger_IsActiveChanged(object sender, EventArgs e)
        {
            UpdateStatesByTriggers();
        }
        public virtual string GroupName { get; }

        protected StateGroup()
        {
        }


        public StateGroup(string groupName)
        {
            GroupName = groupName;
        }

        protected virtual bool IsDefaultState(IStateDefinition stateDefinition)
        {
            return IsDefaultState(stateDefinition?.StateName);
        }
        protected bool IsDefaultState(string stateName)
        {
            return string.IsNullOrWhiteSpace(stateName);
        }

        private async void UpdateStatesByTriggers()
        {
            var current = States.SafeValue(CurrentStateName);
            if (current != null)
            {
                // Don't change state if current state triggers are still active
                if (StateTriggersActive(current)) return;
            }

            var firstActiveState = States.FirstOrDefault(x => StateTriggersActive(x.Value));
            // If there's no active state, then just return
            if (IsDefaultState(firstActiveState.Value)) return;

            if (CurrentStateName.Equals(firstActiveState.Key)) return;

            await ChangeTo(firstActiveState.Key);

        }

        private bool StateTriggersActive(IStateDefinition state)
        {
            return state.Triggers.All(x => x.IsActive);
        }

        public bool TrackHistory { get; set; } = false;

        public bool HasHistory
        {
            get
            {
                if (TrackHistory == false) throw new Exception("History tracking not enabled");
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


        private Stack<string> History { get; } = new Stack<string>();



        //public void TransitionTo<TFindState>(TFindState state) where TFindState : struct
        //{
        //    var visualState = State(state);
        //    visualState?.TransitionTo(DefaultValues);
        //}

        //public void DefineAllStates()
        //{
        //    var vals = Enum.GetValues(typeof(TState));
        //    foreach (var enumVal in vals)
        //    {
        //        $"Defining state {enumVal}".Log();
        //        DefineState((TState)enumVal);
        //    }
        //}

        public virtual IStateDefinition DefineState(IStateDefinition stateDefinition)
        {
            if (stateDefinition == null)
            {
                "Can't define null state definition".Log();
                return null;
            }

            var existing = States.SafeValue(stateDefinition.StateName);
            if (existing != null)
            {
                $"State definition already defined, returning existing instance - {existing.GetType().Name}".Log();
                return existing;
            }

            $"Defining state of type {stateDefinition.GetType().Name}".Log();
            States[stateDefinition.StateName] = stateDefinition;
            return stateDefinition;
        }

        public virtual IStateDefinition DefineState(string state)
        {
            // Don't ever add the default value (eg Base) state
            if (string.IsNullOrWhiteSpace(state))
            {
                "Attempted to add state definition for null or empty state name".Log();
                return null;
            }
            var stateDefinition = new StateDefinition(state);
            return DefineState(stateDefinition);
        }


        public virtual IStateDefinitionWithData<TStateData> DefineStateWithData<TStateData>(string state)
            where TStateData : INotifyPropertyChanged
        {
            // Don't ever add the default value (eg Base) state
            if (string.IsNullOrWhiteSpace(state))
            {
                "Attempted to add state definition for null or empty state name".Log();
                return null;
            }

            $"Defining state for {state}".Log();
            var stateDefinition = new StateDefinition(state)
            {
                UntypedStateDataWrapper = new StateDefinitionTypedDataWrapper<TStateData>()
            };
            var stateDef = DefineState(stateDefinition);
            return new StateDefinitionWithDataWrapper<TStateData> { State = stateDef };
        }


        //public async Task<bool> ChangeTo<TFindState>(TFindState findState, bool useTransitions = true)
        //    where TFindState : struct
        //{
        //    if (typeof (TFindState) != typeof (TState))
        //    {
        //        $"Attempting to change to the wrong state type {typeof (TFindState)}".Log();
        //        return false;
        //    }

        //    var newState = (TState) (object) findState;

        //    return await ChangeTo(newState, useTransitions);
        //}

        public async Task<bool> ChangeToWithData<TData>(string findState, TData data, bool useTransitions = true)
        {
            var dataAsString = data.EncodeJson();
            return await ChangeTo(findState, true, useTransitions, dataAsString);
        }

        //public async Task<bool> ChangeBackTo<TFindState>(TFindState findState, bool useTransitions = true)
        //    where TFindState : struct
        //{
        //    if(TrackHistory==false)throw new Exception("History tracking not enabled");

        //    if (typeof(TFindState) != typeof(TState))
        //    {
        //        $"Attempting to change to the wrong state type {typeof(TFindState)}".Log();
        //        return false;
        //    }

        //    var newState = (TState)(object)findState;

        //    return await ChangeBackTo(newState, useTransitions);
        //}

        public async Task<bool> ChangeToPrevious(bool useTransitions = true)
        {
            if (TrackHistory == false) throw new Exception("History tracking not enabled");

            if (History.Count == 0) return false;

            var previous = History.Peek();

            return await ChangeBackTo(previous, useTransitions);
        }

        public IStateBinder Bind(IStateGroup groupToBindTo, bool bothDirections = true)
        {
            var sg = groupToBindTo;// as IStateGroup<TState>; // This includes INotifyStateChanged
            if (sg == null) return null;

            return new StateGroupBinder(this, sg, bothDirections);
        }

        private class StateGroupBinder : IStateBinder
        {
            private IStateGroup StateGroup { get; }
            private IStateGroup Source { get; }

            private bool BothDirections { get; }
            public StateGroupBinder(IStateGroup sg, IStateGroup source, bool bothDirections)
            {
                BothDirections = bothDirections;
                StateGroup = sg;
                Source = source;

                Source.StateChanged += Source_StateChanged;
                if (bothDirections)
                {
                    var dest = sg;// as IStateGroup<TStateBind>;
                    if (dest == null) return;

                    dest.StateChanged += Dest_StateChanged;
                }
            }

            private void Source_StateChanged(object sender, StateEventArgs e)
            {
                if (e.IsNewState)
                {
                    StateGroup.ChangeTo(e.StateName, e.UseTransitions);
                }
                else
                {
                    StateGroup.ChangeBackTo(e.StateName, e.UseTransitions);
                }
            }

            private void Dest_StateChanged(object sender, StateEventArgs e)
            {
                if (e.IsNewState)
                {
                    Source.ChangeTo(e.StateName, e.UseTransitions);
                }
                else
                {
                    Source.ChangeBackTo(e.StateName, e.UseTransitions);
                }
            }

            public void Unbind()
            {
                Source.StateChanged -= Source_StateChanged;

                if (BothDirections)
                {
                    var dest = StateGroup;// as IStateGroup<TStateBind>;
                    if (dest == null) return;

                    dest.StateChanged += Dest_StateChanged;
                }
            }
        }

        //public async Task<bool> ChangeTo(TState newState, bool useTransitions = true)
        //{
        //    return await ChangeTo(newState, true, useTransitions);
        //}

        //public async Task<bool> ChangeBackTo(TState newState, bool useTransitions = true)
        //{
        //    return await ChangeTo(newState, false, useTransitions);
        //}

        //private async Task<bool> ChangeTo(TState newState, bool isNewState, bool useTransitions, string data = null)
        //{

        //}


        public async Task<bool> ChangeTo(string newState, bool useTransitions = true)
        {
            return await ChangeTo(newState, true, useTransitions);
        }


        public async Task<bool> ChangeBackTo(string newState, bool useTransitions = true)
        {
            return await ChangeTo(newState, false, useTransitions);
        }

        private async Task<bool> ChangeTo(string newState, bool isNewState, bool useTransitions, string data = null)
        {

            $"Changing to state {newState} ({useTransitions})".Log();
            var current = CurrentStateName;
            if (current?.Equals(newState) ?? false)
            {
                "Transitioning to same state - doing nothing".Log();
                return true;
            }


            if (!string.IsNullOrWhiteSpace(current)) //.Equals(default(TState)))
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

            $"About to updated CurrentState (currently: {CurrentStateName})".Log();

            if (isNewState)
            {
                var oldState = CurrentStateName;
                CurrentStateName = newState;
                if (TrackHistory && !IsDefaultState(oldState))
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
                    CurrentStateName = newState;
                    break;
                }
                if (!CurrentStateName.Equals(newState))
                {
                    $"Unable to go back to {newState} as it doesn't exist in the state History".Log();
                    return false;
                }
            }
            $"CurrentState updated (now: {CurrentStateName})".Log();

            States[CurrentStateName].TransitionTo(DefaultValues);

            await NotifyStateChanged(newState, useTransitions, isNewState);

            await ChangedToState(newState, data);

            "ChangeTo completed".Log();
            return true;
        }

#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected virtual async Task NotifyStateChanged(string newState, bool useTransitions, bool isNewState)
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
                        StateChanged.Invoke(this, new StateEventArgs(CurrentStateName, useTransitions, isNewState));
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
        protected virtual async Task<bool> ChangeToState(string oldState, string newState, bool isNewState)
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
            var currentVMStates = !IsDefaultState(oldState) ? States[oldState].UntypedStateDataWrapper : null;
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

            if (!IsDefaultState(oldState))
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
            if (!IsDefaultState(newState))
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
                    // ReSharper disable once SuspiciousTypeConversion.Global - data entities can implement both interfaces
                    (vm as IRegisterDependencies)?.RegisterDependencies(DependencyContainer);

                    await currentVMStates.InvokeInitialise(vm);

                }
                // ReSharper disable once SuspiciousTypeConversion.Global - data entities can implement both interfaces
                var requireUI = vm as IRegisterForUIAccess;
                requireUI?.RegisterForUIAccess(this);

                StateDataCache[vm.GetType()] = vm;
                CurrentStateData = vm;
                // ReSharper disable once SuspiciousTypeConversion.Global - data entities can implement both interfaces
                isBlockable = CurrentStateData as IIsAbleToBeBlocked;
                if (isBlockable != null)
                {
                    isBlockable.IsBlockedChanged += IsBlockable_IsBlockedChanged;
                }
            }


            return true;
        }


#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected virtual async Task ChangedToState(string newState, string dataAsJson)
#pragma warning restore 1998
        {

            var newStateDef = States.SafeValue(newState);
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

            var currentVMStates = States.SafeValue(CurrentStateName)?.UntypedStateDataWrapper;
            if (currentVMStates != null)
            {
                "Invoking ChangedTo on new state definition".Log();
                await currentVMStates.InvokeChangedTo(CurrentStateData);
                await currentVMStates.InvokeChangedToWithData(CurrentStateData, dataAsJson);
            }

        }

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