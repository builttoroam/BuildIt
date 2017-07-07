using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using BuildIt.ServiceLocation;
using BuildIt.States.Interfaces;
using BuildIt.States.Interfaces.StateData;

namespace BuildIt.States
{
    /// <summary>
    /// Defines a group of states
    /// </summary>
    public class StateGroup : IStateGroup
    {
        /// <summary>
        /// Indicates whether the state group is currently able to go to previous has changed
        /// </summary>
        public event EventHandler GoToPreviousStateIsBlockedChanged;

        /// <summary>
        /// Event indicating that the current state for this group has changed
        /// </summary>
        public event EventHandler<StateEventArgs> StateChanged;

        /// <summary>
        /// Event indicating that the current state for this group is about to change
        /// </summary>
        public event EventHandler<StateCancelEventArgs> StateChanging;

        /// <summary>
        /// Gets or sets dependency container for registering and retrieving types
        /// </summary>
        public IDependencyContainer DependencyContainer { get; set; }

        /// <summary>
        /// Gets or sets context for doing UI tasks
        /// </summary>
        public IUIExecutionContext UIContext { get; set; }

        /// <summary>
        /// Gets or sets the name (unique in this group) of the current state
        /// </summary>
        public virtual string CurrentStateName { get; protected set; }


        /// <summary>
        /// Gets returns the state definition for the current state
        /// </summary>
        public IStateDefinition CurrentStateDefinition
            => !IsDefaultState(CurrentStateName) ? States.SafeValue(CurrentStateName) : null;

        /// <summary>
        /// Gets returns information about the data entity associated with the current state
        /// </summary>
        public IStateDefinitionDataWrapper CurrentStateDataWrapper => !IsDefaultState(CurrentStateName)
            ? CurrentStateDefinition?.UntypedStateDataWrapper
            : null;

        /// <summary>
        /// Retrieve state definition based on state name
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public IStateDefinition StateDefinition(string state)
        {
            if (string.IsNullOrWhiteSpace(state))
            {
                return null;
            }

            return States.SafeValue(state);
        }

        /// <summary>
        /// Gets dictionary of states that can be transitioned to
        /// </summary>
        public IDictionary<string, IStateDefinition> States { get; } =
            new Dictionary<string, IStateDefinition>();

        /// <summary>
        /// Gets internal dictionary of default property values - so that they can be
        /// unset in the case of transitioning to a state that doesn't define
        /// values for every property
        /// </summary>
        private IDictionary<Tuple<object, string>, IDefaultValue> DefaultValues { get; }
            = new Dictionary<Tuple<object, string>, IDefaultValue>();


        /// <summary>
        /// Gets cache of state data entities
        /// </summary>
        private IDictionary<Type, INotifyPropertyChanged> StateDataCache { get; } =
           new Dictionary<Type, INotifyPropertyChanged>();

        /// <summary>
        /// Gets or sets the current state data
        /// </summary>
        public INotifyPropertyChanged CurrentStateData { get; set; }

        /// <summary>
        /// Retrieves any existing entity for a particular entity type
        /// </summary>
        /// <param name="stateDataType"></param>
        /// <returns></returns>
        private INotifyPropertyChanged Existing(Type stateDataType) =>
            stateDataType == null ? null : StateDataCache.SafeValue(stateDataType);

        /// <summary>
        /// Gets all triggers defined for the states in this group
        /// </summary>
        private IList<IStateTrigger> Triggers { get; } = new List<IStateTrigger>();

        /// <summary>
        /// Gets the name of the state group
        /// </summary>
        public virtual string GroupName { get; }

        /// <summary>
        /// Gets or sets a value indicating whether /// Whether history will be recorded for this state group
        /// </summary>
        public bool TrackHistory { get; set; } = false;

        /// <summary>
        /// Gets the history stack of state names
        /// </summary>
        private Stack<string> History { get; } = new Stack<string>();

        /// <summary>
        /// Gets a value indicating whether whether history has been recorded
        /// </summary>
        public bool HasHistory
        {
            get
            {
                if (TrackHistory == false)
                {
                    throw new Exception("History tracking not enabled");
                }

                return History.Count > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether whether the current state is preventing go to previous
        /// Note: History must be enabled!
        /// </summary>
        public virtual bool GoToPreviousStateIsBlocked
        {
            get
            {
                // Always block going to previous if history not enabled
                if (!TrackHistory)
                {
                    return true;
                }

                // ReSharper disable once SuspiciousTypeConversion.Global
                var isBlockable = CurrentStateData as IIsAbleToBeBlocked;
                return isBlockable?.IsBlocked ?? false;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="StateGroup"/> class.
        /// Internal constructor to limit construction without providing a name
        /// </summary>
        protected StateGroup()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateGroup"/> class.
        /// Constructs a group based on the supplied group name
        /// </summary>
        /// <param name="groupName">The name of the state group</param>
        public StateGroup(string groupName)
        {
            GroupName = groupName;
        }


        /// <summary>
        /// Add and start watching a state trigger
        /// </summary>
        /// <param name="trigger">The state trigger to watch</param>
        public void WatchTrigger(IStateTrigger trigger)
        {
            trigger.IsActiveChanged += Trigger_IsActiveChanged;
            Triggers.Add(trigger);
        }

        private void Trigger_IsActiveChanged(object sender, EventArgs e)
        {
            UpdateStatesByTriggers();
        }

        /// <summary>
        /// Determines whether the supplied state definition is the default state
        /// </summary>
        /// <param name="stateDefinition">The state definition</param>
        /// <returns></returns>
        protected bool IsDefaultState(IStateDefinition stateDefinition)
        {
            return IsDefaultState(stateDefinition?.StateName);
        }

        /// <summary>
        /// Determines whether the supplied state name is the default state
        /// </summary>
        /// <param name="stateName">The state name</param>
        /// <returns></returns>
        protected bool IsDefaultState(string stateName)
        {
            return string.IsNullOrWhiteSpace(stateName);
        }

        private async void UpdateStatesByTriggers()
        {
            // Don't change state if current state triggers are still active
            if (CurrentStateDefinition.AllTriggersActive())
            {
                return;
            }

            var firstActiveState = States.FirstOrDefault(x => x.Value.AllTriggersActive());
            // If there's no active state, then just return
            if (IsDefaultState(firstActiveState.Value))
            {
                return;
            }

            if (CurrentStateName == firstActiveState.Key)
            {
                return;
            }

            await ChangeTo(firstActiveState.Key);
        }

        private void IsBlockable_IsBlockedChanged(object sender, EventArgs e)
        {
            OnGoToPreviousStateIsBlockedChanged();
        }


        /// <summary>
        /// Method for raising the event indicating that going to previous has changed
        /// </summary>
        protected void OnGoToPreviousStateIsBlockedChanged()
        {
            GoToPreviousStateIsBlockedChanged.SafeRaise(this);
        }

        /// <summary>
        /// Add the state definition to the list of definitions. Returns the
        /// existing state if one already exists with the same name
        /// </summary>
        /// <param name="stateDefinition">The state definition to register</param>
        /// <returns>The registered state definition</returns>
        public virtual IStateDefinition DefineState(IStateDefinition stateDefinition)
        {
            if (stateDefinition == null)
            {
                "Can't define null state definition".Log();
                return null;
            }

            var existing = StateDefinition(stateDefinition.StateName);// States.SafeValue(stateDefinition.StateName);
            if (existing != null)
            {
                $"State definition already defined, returning existing instance - {existing.GetType().Name}".Log();
                return existing;
            }

            $"Defining state of type {stateDefinition.GetType().Name}".Log();
            States[stateDefinition.StateName] = stateDefinition;
            return stateDefinition;
        }

        /// <summary>
        /// Create state definition
        /// </summary>
        /// <param name="state">The name of the state definition to create</param>
        /// <returns></returns>
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

        /// <summary>
        /// Define a state with data entity
        /// </summary>
        /// <typeparam name="TStateData">The type of the data to be associated with the state</typeparam>
        /// <param name="state">The name of the state</param>
        /// <returns>Wrapper around the state definition and data wrapper</returns>
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
            var stateDef = DefineState(new StateDefinition(state));
            if (stateDef is StateDefinition stateDefinition)
            {
                stateDefinition.UntypedStateDataWrapper = new StateDefinitionTypedDataWrapper<TStateData>();
            }

            return new StateDefinitionWithDataWrapper<TStateData> { State = stateDef };
        }

        /// <summary>
        /// Change to a new state
        /// </summary>
        /// <param name="newState">The name of the state to change to</param>
        /// <param name="useTransitions">Should use transitions</param>
        /// <returns>Success indicator</returns>
        public async Task<bool> ChangeTo(string newState, bool useTransitions = true)
        {
            return await PerformStateChange(newState, true, useTransitions);
        }

        /// <summary>
        /// Change to state by going back
        /// </summary>
        /// <param name="newState">The name of the state to change to</param>
        /// <param name="useTransitions">Should use transitions</param>
        /// <returns>Success indicator</returns>
        public async Task<bool> ChangeBackTo(string newState, bool useTransitions = true)
        {
            if (TrackHistory == false)
            {
                throw new Exception("History tracking not enabled");
            }

            return await PerformStateChange(newState, false, useTransitions);
        }

        /// <summary>
        /// Change to new state
        /// </summary>
        /// <typeparam name="TData">The type of data to be passed in</typeparam>
        /// <param name="findState">The state to transition to</param>
        /// <param name="data">The data to be passed in</param>
        /// <param name="useTransitions">Should use transitions</param>
        /// <returns>Success indicator</returns>
        public async Task<bool> ChangeToWithData<TData>(string findState, TData data, bool useTransitions = true)
        {
            var dataAsString = data.EncodeJson();
            return await PerformStateChange(findState, true, useTransitions, dataAsString);
        }

        /// <summary>
        /// Change to the previous state
        /// </summary>
        /// <param name="useTransitions">Should use transitions</param>
        /// <returns>Success indicator</returns>
        public async Task<bool> ChangeToPrevious(bool useTransitions = true)
        {
            if (History.Count == 0)
            {
                return false;
            }

            var previous = History.Peek();

            return await ChangeBackTo(previous, useTransitions);
        }

        /// <summary>
        /// Binds one state group to another
        /// </summary>
        /// <param name="groupToBindTo">The source group (ie changes in the source group update this group)</param>
        /// <param name="bothDirections">Whether updates should flow both directions</param>
        /// <returns>Binder entity that manages the relationship</returns>
        public IStateBinder Bind(IStateGroup groupToBindTo, bool bothDirections = true)
        {
            var sg = groupToBindTo;// as IStateGroup<TState>; // This includes INotifyStateChanged
            if (sg == null)
            {
                return null;
            }

            return new StateGroupBinder(this, sg, bothDirections);
        }

        private bool IsCurrentState(string newState)
        {
            // Check to see whether this is a change to the same state
            var current = CurrentStateName;
            return current?.Equals(newState) ?? false;
        }

        /// <summary>
        /// Change to new state
        /// </summary>
        /// <param name="newState">The name of the new state</param>
        /// <param name="isNewState">Whether this is a new state, or change to previous</param>
        /// <param name="useTransitions">Use transition</param>
        /// <param name="data">Json data to pass into the new state</param>
        /// <returns>Indicates successful transition to the newState</returns>
        private async Task<bool> PerformStateChange(string newState, bool isNewState, bool useTransitions, string data = null)
        {

            // Check to see whether this is a change to the same state
            if (IsCurrentState(newState))
            {
                "Transitioning to same state - doing nothing".Log();
                return true;
            }

            var current = CurrentStateName;
            $"Changing state from {current} to {newState} (Transitions: {useTransitions})".Log();

            // Invoke all the methods/events prior to changing state (cancellable!)
            "Invoking AboutToChangeFrom to confirm state change can proceed".Log();
            var success = await AboutToChangeFrom(newState, data, isNewState, useTransitions);
            if (!success)
            {
                return false;
            }

            // Invoke changing methods - not cancellable but allows freeing up resources/event handlers etc
            "Invoking ChangingFrom before state change".Log();
            await ChangingFrom(newState, data, isNewState, useTransitions);

            // Perform the state change
            "Invoking ChangeCurrentState to perform state change".Log();
            await ChangeCurrentState(newState, isNewState, useTransitions);

            // Perform post-change methods
            "Invoking ChangedToState after state change".Log();
            await ChangedToState(newState, data, isNewState, useTransitions);

            "ChangeTo completed".Log();
            return true;
        }


        /// <summary>
        /// Invokes methods allowing change of state to be cancelled:
        /// - Notify that state is changing
        /// - State definition
        /// - State definition data wrapper
        /// - State data
        /// Note: If overriding, make sure to call the base method!
        /// </summary>
        /// <param name="newState">The new state to transition to</param>
        /// <param name="data">The json data to be passed to the new state</param>
        /// <param name="isNewState">Is this a new state (forward) or going back</param>
        /// <param name="useTransitions">Should use transitions</param>
        /// <returns>Success indicator</returns>
        protected virtual async Task<bool> AboutToChangeFrom(string newState, string data, bool isNewState, bool useTransitions)
        {
            var current = CurrentStateName;
            var currentStateDef = CurrentStateDefinition;
            var currentStateDataWrapper = CurrentStateDataWrapper;

            // Raise an event before changing state
            var cancelChange = await NotifyStateChanging(current, isNewState, useTransitions);
            if (cancelChange)
            {
                return false;
            }

            var cancel = new CancelEventArgs();

            // Invoke the cancel method associated with the state definition
            if (currentStateDef?.AboutToChangeFrom != null)
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

            "Retrieving current state definition".Log();
            if (currentStateDataWrapper != null)
            {
                "Invoking AboutToChangeFrom for existing state definition".Log();
                await currentStateDataWrapper.InvokeAboutToChangeFrom(CurrentStateData, cancel);
                if (cancel.Cancel)
                {
                    "ChangeToState cancelled by existing state definition".Log();
                    return false;
                }
            }

            // ReSharper disable once SuspiciousTypeConversion.Global - NOT HELPFUL
            if (CurrentStateData is IAboutToChangeFrom stateData)
            {
                "Invoking AboutToLeave".Log();
                await stateData.AboutToChangeFrom(cancel);
                if (cancel.Cancel)
                {
                    "ChangeToState cancelled by AboutToLeave".Log();
                    return false;
                }
            }

            var newStateDef = StateDefinition(newState);
            if (newStateDef?.AboutToChangeTo != null)
            {
                "Invoking 'AboutToChangeTo' on current state definition".Log();
                await newStateDef.AboutToChangeTo(cancel);
                "'AboutToChangeTo' completed".Log();
                if (cancel.Cancel)
                {
                    "Cancelling state transition invoking 'AboutToChangeTo'".Log();
                    return false;
                }
            }

            if (newStateDef?.AboutToChangeToWithData != null)
            {
                "Invoking 'AboutToChangeTo' on current state definition".Log();
                await newStateDef.AboutToChangeToWithData(data, cancel);
                "'AboutToChangeTo' completed".Log();
                if (cancel.Cancel)
                {
                    "Cancelling state transition invoking 'AboutToChangeTo'".Log();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Invokes methods as state is about to change (ie ending current state)
        /// - State definition
        /// - State definition data wrapper
        /// - State data
        /// Note: If overriding, make sure to call the base method!
        /// </summary>
        /// <param name="newState">The new state</param>
        /// <param name="dataAsJson">The data to be passed into the new state</param>
        /// <param name="isNewState">Is this a new state (forward) or going back</param>
        /// <param name="useTransitions">Should use transitions</param>
        protected virtual async Task ChangingFrom(string newState, string dataAsJson, bool isNewState, bool useTransitions)
        {
            var currentStateDef = CurrentStateDefinition;
            var currentStateDataWrapper = CurrentStateDataWrapper;

            if (currentStateDef?.ChangingFrom != null)
            {
                "Invoking 'ChangingFrom'".Log();
                await currentStateDef.ChangingFrom();
            }

            if (currentStateDataWrapper != null)
            {
                "Invoking ChangingFrom on current state definition".Log();
                await currentStateDataWrapper.InvokeChangingFrom(CurrentStateData);
            }

            // ReSharper disable once SuspiciousTypeConversion.Global // NOT HELPFUL
            if (CurrentStateData is IChangingFrom leaving)
            {
                "Invoking Leaving on current view model".Log();
                await leaving.ChangingFrom();
            }

            var newStateDef = StateDefinition(newState);
            var hasData = !string.IsNullOrWhiteSpace(dataAsJson);

            if (newStateDef?.ChangingTo != null)
            {
                "Invoking 'ChangingTo' on new state definition".Log();
                await newStateDef.ChangingTo();
            }

            if (hasData && newStateDef?.ChangingToWithData != null)
            {
                "Invoking 'ChangingToWithData' on new state definition".Log();
                await newStateDef.ChangingToWithData(dataAsJson);
            }
        }

        /// <summary>
        /// Change to the new state
        /// Note: If overriding, make sure to call the base method!
        /// </summary>
        /// <param name="newState">The name of the new state to transition to</param>
        /// <param name="isNewState">Is this a new state (forward) or going back</param>
        /// <param name="useTransitions">Should use transitions</param>
#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected virtual async Task ChangeCurrentState(string newState, bool isNewState, bool useTransitions)
#pragma warning restore 1998
        {
            // ReSharper disable once SuspiciousTypeConversion.Global // NOT HELPFUL
            if (CurrentStateData is IIsAbleToBeBlocked isBlockable)
            {
                "Detaching event handlers for isblocked on current view model".Log();
                isBlockable.IsBlockedChanged -= IsBlockable_IsBlockedChanged;
            }

            var newStateDef = States.SafeValue(newState);
            var newStateDataWrapper = newStateDef?.UntypedStateDataWrapper;


            if (newStateDataWrapper != null)
            {

                "Retrieving existing ViewModel for new state".Log();
                var stateData = Existing(newStateDataWrapper.StateDataType);
                if (stateData == null)
                {
                    //var newGen = States[newState] as IGenerateViewModel;
                    //if (newGen == null) return false;
                    "Generating ViewModel for new state".Log();
                    stateData = newStateDataWrapper.Generate();

                    "Registering dependencies".Log();
                    // ReSharper disable once SuspiciousTypeConversion.Global - data entities can implement both interfaces
                    (stateData as IRegisterDependencies)?.RegisterDependencies(DependencyContainer);

                    await newStateDataWrapper.InvokeInitialise(stateData);
                }
                // ReSharper disable once SuspiciousTypeConversion.Global - data entities can implement both interfaces
                (stateData as IRegisterForUIAccess)?.RegisterForUIAccess(this);

                StateDataCache[stateData.GetType()] = stateData;
                CurrentStateData = stateData;
                // ReSharper disable once SuspiciousTypeConversion.Global - data entities can implement both interfaces
                isBlockable = CurrentStateData as IIsAbleToBeBlocked;
                if (isBlockable != null)
                {
                    isBlockable.IsBlockedChanged += IsBlockable_IsBlockedChanged;
                }
            }
            else
            {
                CurrentStateData = null;
            }

            // Update current state as well as history
            $"About to updated CurrentState (currently: {CurrentStateName})".Log();
            if (isNewState)
            {
                // New state - so add to the history
                var oldState = CurrentStateName;
                CurrentStateName = newState;
                if (TrackHistory && !IsDefaultState(oldState))
                {
                    History.Push(oldState);
                }
            }
            else
            {
                // Not new state (ie go back) to keep
                // popping off history until we find the newState
                while (History.Count > 0)
                {
                    var historyState = History.Pop();
                    if (!historyState.Equals(newState))
                    {
                        continue;
                    }

                    CurrentStateName = newState;
                    break;
                }
                if (!(CurrentStateName?.Equals(newState) ?? false))
                {
                    CurrentStateName = newState;
                    $"Unable to go back to {newState} as it doesn't exist in the state History".Log();
                }
            }
            $"CurrentState updated (now: {CurrentStateName})".Log();

            // Perform state transitions - adjust all the properties etc
            CurrentStateDefinition?.TransitionTo(DefaultValues);
        }


        /// <summary>
        /// Invokes methods after state change
        /// - State data
        /// - State definition data wrapper
        /// - State definition
        /// Note: If overriding, make sure to call the base method!
        /// </summary>
        /// <param name="oldState">The previous/existing state</param>
        /// <param name="dataAsJson">Any data that needs to be passed in</param>
        /// <param name="isNewState">Is this a new state (forward) or going back</param>
        /// <param name="useTransitions">Should use transitions</param>
#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected virtual async Task ChangedToState(string oldState, string dataAsJson, bool isNewState, bool useTransitions)
#pragma warning restore 1998
        {
            var oldStateDef = StateDefinition(oldState);
            if (oldStateDef?.ChangedFrom != null)
            {
                "Invoking ChangedFrom on old state definition".Log();
                await oldStateDef.ChangedFrom();
            }

            var oldStateDataWrapper = oldStateDef?.UntypedStateDataWrapper;
            if (oldStateDataWrapper != null)
            {
                "Invoking ChangedFrom on current state definition".Log();
                await oldStateDataWrapper.InvokeChangedFrom(CurrentStateData);
            }



            var currentStateDef = CurrentStateDefinition;
            var currentStateDataWrapper = CurrentStateDataWrapper;

            var hasData = !string.IsNullOrWhiteSpace(dataAsJson);

            // ReSharper disable once SuspiciousTypeConversion.Global // NOT HELPFUL
            if (CurrentStateData is IChangedTo arrived)
            {
                "Invoking Arriving on new ViewModel".Log();
                await arrived.ChangedTo();
            }

            // ReSharper disable once SuspiciousTypeConversion.Global // NOT HELPFUL
            if (hasData && CurrentStateData is IChangedToWithData arrivedWithData)
            {
                "Invoking Arriving on new ViewModel".Log();
                await arrivedWithData.ChangedToWithData(dataAsJson);
            }

            if (currentStateDef?.ChangedTo != null)
            {
                $"State definition found, of type {currentStateDef.GetType().Name}, invoking ChangedTo method".Log();
                await currentStateDef.ChangedTo();
                "ChangedTo completed".Log();
            }
            else
            {
                "No new state definition".Log();
            }

            if (hasData && currentStateDef?.ChangedToWithJsonData != null)
            {
                $"State definition found, of type {currentStateDef.GetType().Name}, invoking ChangedToWithJsonData method".Log();
                await currentStateDef.ChangedToWithJsonData(dataAsJson);
                "ChangedToWithJsonData completed".Log();
            }
            else
            {
                "No new state definition".Log();
            }

            if (currentStateDataWrapper != null)
            {
                "Invoking ChangedTo on new state definition".Log();
                await currentStateDataWrapper.InvokeChangedTo(CurrentStateData);

                if (hasData)
                {
                    await currentStateDataWrapper.InvokeChangedToWithData(CurrentStateData, dataAsJson);
                }
            }

            // Raise event after state changed
            await NotifyStateChanged(CurrentStateName, isNewState, useTransitions);
        }

        /// <summary>
        /// Overridable method to raise StateChanged event
        /// </summary>
        /// <param name="newState">The name of the state being changed to</param>
        /// <param name="isNewState">Whether the new state is a new state or being returned to</param>
        /// <param name="useTransitions">Indicates whether to use transitions</param>
        /// <returns></returns>
#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected virtual async Task NotifyStateChanged(string newState, bool isNewState, bool useTransitions)
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
                        StateChanged?.Invoke(this, new StateEventArgs(CurrentStateName, useTransitions, isNewState));
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


        /// <summary>
        /// Overridable method to raise the StateChanging event
        /// </summary>
        /// <param name="newState">The new state to transition to</param>
        /// <param name="isNewState">Whether this will be a new state or going to previous</param>
        /// <param name="useTransitions">Whether to use transitions or not</param>
        /// <returns>Whether the state change should be cancelled (true)</returns>
#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected virtual async Task<bool> NotifyStateChanging(string newState, bool isNewState, bool useTransitions)
#pragma warning restore 1998
        {
            var shouldCancel = false;
            try
            {
                if (StateChanging != null)
                {
                    "Invoking StateChanging event (before UI context check)".Log();
                    await UIContext.RunAsync(() =>
                    {
                        var cancel = new StateCancelEventArgs(CurrentStateName, useTransitions, isNewState);
                        "Raising StateChanging event".Log();
                        StateChanging?.Invoke(this, cancel);
                        "Raising StateChanging event completed".Log();
                        shouldCancel = cancel.Cancel;
                    });
                    "StateChanging event completed (after UI context check)".Log();
                }
                else
                {
                    "Nothing listening to StateChanging".Log();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
                // Ignore any errors caused by the event being raised, as
                // the state change has still occurred
            }
            return shouldCancel;
        }


        /// <summary>
        /// Registers any depedencies, including in the defined states and data wrappers
        /// </summary>
        /// <param name="container"></param>
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