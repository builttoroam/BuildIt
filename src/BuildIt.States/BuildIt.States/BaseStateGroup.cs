using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BuildIt.ServiceLocation;
using BuildIt.States.Interfaces;
using BuildIt.States.Interfaces.StateData;

namespace BuildIt.States
{
    /// <summary>
    /// Defines a group of states
    /// </summary>
    /// <typeparam name="TStateDefinition">Type of state definition</typeparam>
    /// <typeparam name="TStateGroupDefinition">Type of group definition</typeparam>
    public abstract class BaseStateGroup<TStateDefinition, TStateGroupDefinition>
        : IStateGroup<TStateDefinition, TStateGroupDefinition>
        where TStateDefinition : class, IStateDefinition, new()
        where TStateGroupDefinition : class, IStateGroupDefinition<TStateDefinition>, new()
    {
        private string currentStateName;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseStateGroup{TStateDefinition, TStateGroupDefinition}"/> class.
        /// </summary>
        /// <param name="cacheKey">The cacheKey for the state definition</param>
        protected BaseStateGroup(string cacheKey = null)
            : this(CachedOrNewGroupDefinitionByKey(cacheKey))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseStateGroup{TStateDefinition, TStateGroupDefinition}"/> class.
        /// </summary>
        /// <param name="groupDefinition">The definition for this stage group</param>
        protected BaseStateGroup(TStateGroupDefinition groupDefinition)
        {
            TypedGroupDefinition = groupDefinition;
        }

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
        /// Gets the state group definition (including the states that make up the group)
        /// </summary>
        public IStateGroupDefinition GroupDefinition => TypedGroupDefinition;

        /// <summary>
        /// Gets or sets the state group definition (including the states that make up the group)
        /// </summary>
        public TStateGroupDefinition TypedGroupDefinition { get; set; }

        /// <summary>
        /// Gets the name of the state group
        /// </summary>
        public string GroupName => TypedGroupDefinition.GroupName;

        /// <summary>
        /// Gets or sets dependency container for registering and retrieving types
        /// </summary>
        public IDependencyContainer DependencyContainer { get; set; }

        /// <summary>
        /// Gets or sets context for doing UI tasks
        /// </summary>
        public IUIExecutionContext UIContext { get; set; }

        /// <summary>
        /// Gets or sets the cancellation for the current state transition
        /// </summary>
        private CancellationTokenSource StateTransitionCancellation { get; set; }

        /// <summary>
        /// Gets a semaphore to block concurrent state transitions
        /// </summary>
        private SemaphoreSlim StateTransitionSemaphore { get; } = new SemaphoreSlim(1);

        /// <summary>
        /// Gets a lock protecting access to the state transition cancellation source
        /// </summary>
        private object CancellationLock { get; } = new object();

        /// <summary>
        /// Gets or sets the current state name
        /// </summary>
        public virtual string CurrentStateName
        {
            get => currentStateName;
            protected set
            {
                CurrentTypedStateDefinition = (from s in GroupDefinition.States
                                               let def = s.Value
                                               where def?.StateName == value
                                               select def).FirstOrDefault() as TStateDefinition;
                if (CurrentTypedStateDefinition != null)
                {
                    currentStateName = CurrentTypedStateDefinition.StateName;
                }
            }
        }

        /// <summary>
        /// Gets returns the state definition for the current state
        /// </summary>
        public IStateDefinition CurrentStateDefinition => CurrentTypedStateDefinition;

        /// <summary>
        /// Gets or sets gets returns the state definition for the current state
        /// </summary>
        public TStateDefinition CurrentTypedStateDefinition { get; protected set; }

        /// <summary>
        /// Gets returns information about the data entity associated with the current state
        /// </summary>
        public IStateDefinitionDataWrapper CurrentStateDataWrapper => !IsDefaultState(CurrentStateName)
            ? CurrentStateDefinition?.UntypedStateDataWrapper
            : null;

        /// <summary>
        /// Gets or sets the current state data
        /// </summary>
        public INotifyPropertyChanged CurrentStateData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether /// Whether history will be recorded for this state group
        /// </summary>
        public bool TrackHistory { get; set; } = false;

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
        /// Gets the targets to be used when changing state
        /// </summary>
        public IDictionary<string, object> StateValueTargets { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets cache of state data entities
        /// </summary>
        protected IDictionary<Type, INotifyPropertyChanged> StateDataCache { get; } =
            new Dictionary<Type, INotifyPropertyChanged>();

        private static IDictionary<string, TStateGroupDefinition> CachedGroupDefinitions { get; } = new Dictionary<string, TStateGroupDefinition>();

        /// <summary>
        /// Gets all triggers defined for the states in this group
        /// </summary>
        private IList<IStateTrigger> Triggers { get; } = new List<IStateTrigger>();

        /// <summary>
        /// Gets the history stack of state names
        /// </summary>
        private Stack<string> History { get; } = new Stack<string>();

        /// <summary>
        /// Add and start watching a state trigger
        /// </summary>
        /// <param name="trigger">The state trigger to watch</param>
        public void WatchTrigger(IStateTrigger trigger)
        {
            trigger.IsActiveChanged += Trigger_IsActiveChanged;
            Triggers.Add(trigger);
        }

        /// <summary>
        /// Change to a new state
        /// </summary>
        /// <param name="newState">The name of the state to change to</param>
        /// <param name="useTransitions">Should use transitions</param>
        /// <returns>Success indicator</returns>
        public Task<bool> ChangeToStateByName(string newState, bool useTransitions = true)
        {
            return ChangeToStateByName(newState, useTransitions, CancellationToken.None);
        }

        /// <summary>
        /// Change to a new state
        /// </summary>
        /// <param name="newState">The name of the state to change to</param>
        /// <param name="useTransitions">Should use transitions</param>
        /// <param name="cancel">Cancellation token allowing change to be cancelled</param>
        /// <returns>Success indicator</returns>
        public async Task<bool> ChangeToStateByName(string newState, bool useTransitions, CancellationToken cancel)
        {
            return await PerformStateChange(newState, true, useTransitions, null, cancel);
        }

        /// <summary>
        /// Change to state by going back
        /// </summary>
        /// <param name="newState">The name of the state to change to</param>
        /// <param name="useTransitions">Should use transitions</param>
        /// <returns>Success indicator</returns>
        public Task<bool> ChangeBackToStateByName(string newState, bool useTransitions = true)
        {
            return ChangeBackToStateByName(newState, useTransitions, CancellationToken.None);
        }

        /// <summary>
        /// Change to state by going back
        /// </summary>
        /// <param name="newState">The name of the state to change to</param>
        /// <param name="useTransitions">Should use transitions</param>
        /// <param name="cancel">Cancellation token allowing change to be cancelled</param>
        /// <returns>Success indicator</returns>
        public async Task<bool> ChangeBackToStateByName(string newState, bool useTransitions, CancellationToken cancel)
        {
            if (TrackHistory == false)
            {
                throw new Exception("History tracking not enabled");
            }

            if (GoToPreviousStateIsBlocked)
            {
                "Can't go back as is being blocked".Log();
                return false;
            }

            return await PerformStateChange(newState, false, useTransitions, null, cancel);
        }

        /// <summary>
        /// Change to new state
        /// </summary>
        /// <typeparam name="TData">The type of data to be passed in</typeparam>
        /// <param name="findState">The state to transition to</param>
        /// <param name="data">The data to be passed in</param>
        /// <param name="useTransitions">Should use transitions</param>
        /// <returns>Success indicator</returns>
        public Task<bool> ChangeToStateByNameWithData<TData>(string findState, TData data, bool useTransitions = true)
        {
            return ChangeToStateByNameWithData(findState, data, useTransitions, CancellationToken.None);
        }

        /// <summary>
        /// Change to new state
        /// </summary>
        /// <typeparam name="TData">The type of data to be passed in</typeparam>
        /// <param name="findState">The state to transition to</param>
        /// <param name="data">The data to be passed in</param>
        /// <param name="useTransitions">Should use transitions</param>
        /// <param name="cancel">Cancellation token allowing change to be cancelled</param>
        /// <returns>Success indicator</returns>
        public async Task<bool> ChangeToStateByNameWithData<TData>(string findState, TData data, bool useTransitions, CancellationToken cancel)
        {
            var dataAsString = data.EncodeJson();
            return await PerformStateChange(findState, true, useTransitions, dataAsString, cancel);
        }

        /// <summary>
        /// Change to the previous state
        /// </summary>
        /// <param name="useTransitions">Should use transitions</param>
        /// <returns>Success indicator</returns>
        public Task<bool> ChangeToPrevious(bool useTransitions = true)
        {
            return ChangeToPrevious(useTransitions, CancellationToken.None);
        }

        /// <summary>
        /// Change to the previous state
        /// </summary>
        /// <param name="useTransitions">Should use transitions</param>
        /// <param name="cancel">Cancellation token allowing change to be cancelled</param>
        /// <returns>Success indicator</returns>
        public async Task<bool> ChangeToPrevious(bool useTransitions, CancellationToken cancel)
        {
            if (History.Count == 0)
            {
                return false;
            }

            var previous = History.Peek();

            return await ChangeBackToStateByName(previous, useTransitions);
        }

        /// <summary>
        /// Binds one state group to another
        /// </summary>
        /// <param name="groupToBindTo">The source group (ie changes in the source group update this group)</param>
        /// <param name="bothDirections">Whether updates should flow both directions</param>
        /// <returns>Binder entity that manages the relationship</returns>
        public virtual async Task<IStateBinder> Bind(IStateGroup groupToBindTo, bool bothDirections = true)
        {
            var sg = groupToBindTo; // as IStateGroup<TState>; // This includes INotifyStateChanged
            if (sg == null)
            {
                return null;
            }

            var binder = new StateGroupBinder(this, sg, bothDirections);
            await binder.Bind();
            return binder;
        }

        /// <summary>
        /// Registers any depedencies, including in the defined states and data wrappers
        /// </summary>
        /// <param name="container">The container to register dependencies into</param>
        public virtual void RegisterDependencies(IDependencyContainer container)
        {
            DependencyContainer = container;
            using (container.StartUpdate())
            {
                foreach (var state in GroupDefinition.States.Values.Select(x => x.UntypedStateDataWrapper).Where(x => x != null))
                {
                    container.RegisterType(state.StateDataType);
                }
            }
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
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled</param>
        /// <returns>Success indicator</returns>
        protected virtual async Task<bool> AboutToChangeFrom(string newState, string data, bool isNewState, bool useTransitions, CancellationToken cancelToken)
        {
            var current = CurrentStateName;
            var currentStateDef = CurrentStateDefinition;
            var currentStateDataWrapper = CurrentStateDataWrapper;

            // Raise an event before changing state
            var cancelChange = await NotifyStateChanging(current, isNewState, useTransitions, cancelToken);
            if (cancelChange || cancelToken.IsCancellationRequested)
            {
                return false;
            }

            var cancel = new StateCancelEventArgs(newState, useTransitions, isNewState, cancelToken);

            // Invoke the cancel method associated with the state definition
            if (currentStateDef?.AboutToChangeFrom != null)
            {
                "Invoking 'AboutToChangeFrom' on current state definition".Log();
                await currentStateDef.AboutToChangeFrom(cancel).SafeAwait();
                "'AboutToChangeFrom' completed".Log();
            }

            if (cancel.Cancel || cancelToken.IsCancellationRequested)
            {
                "Cancelling state transition invoking 'AboutToChangeFrom'".Log();
                return false;
            }

            "Retrieving current state definition".Log();
            if (currentStateDataWrapper != null)
            {
                "Invoking AboutToChangeFrom for existing state definition".Log();
                await currentStateDataWrapper.InvokeAboutToChangeFrom(CurrentStateData, cancel).SafeAwait();
                if (cancel.Cancel || cancelToken.IsCancellationRequested)
                {
                    "ChangeToState cancelled by existing state definition".Log();
                    return false;
                }
            }

            // ReSharper disable once SuspiciousTypeConversion.Global - NOT HELPFUL
            if (CurrentStateData is IAboutToChangeFrom stateData)
            {
                "Invoking AboutToLeave".Log();
                await stateData.AboutToChangeFrom(cancel).SafeAwait();
                if (cancel.Cancel || cancelToken.IsCancellationRequested)
                {
                    "ChangeToState cancelled by AboutToLeave".Log();
                    return false;
                }
            }

            var newStateDef = GroupDefinition.StateDefinitionFromName(newState);
            if (newStateDef?.AboutToChangeTo != null)
            {
                "Invoking 'AboutToChangeTo' on current state definition".Log();
                await newStateDef.AboutToChangeTo(cancel).SafeAwait();
                "'AboutToChangeTo' completed".Log();
                if (cancel.Cancel || cancelToken.IsCancellationRequested)
                {
                    "Cancelling state transition invoking 'AboutToChangeTo'".Log();
                    return false;
                }
            }

            if (newStateDef?.AboutToChangeToWithData != null)
            {
                "Invoking 'AboutToChangeTo' on current state definition".Log();
                await newStateDef.AboutToChangeToWithData(data, cancel).SafeAwait();
                "'AboutToChangeTo' completed".Log();
                if (cancel.Cancel || cancelToken.IsCancellationRequested)
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
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled</param>
        /// <returns>Task to be awaited</returns>
        protected virtual async Task ChangingFrom(string newState, string dataAsJson, bool isNewState, bool useTransitions, CancellationToken cancelToken)
        {
            var currentStateDef = CurrentStateDefinition;
            var currentStateDataWrapper = CurrentStateDataWrapper;

            if (currentStateDef?.ChangingFrom != null)
            {
                "Invoking 'ChangingFrom'".Log();
                await currentStateDef.ChangingFrom(cancelToken).SafeAwait();
            }

            if (currentStateDataWrapper != null)
            {
                "Invoking ChangingFrom on current state definition".Log();
                await currentStateDataWrapper.InvokeChangingFrom(CurrentStateData, cancelToken).SafeAwait();
            }

            // ReSharper disable once SuspiciousTypeConversion.Global // NOT HELPFUL
            if (CurrentStateData is IChangingFrom leaving)
            {
                "Invoking Leaving on current view model".Log();
                await leaving.ChangingFrom(cancelToken).SafeAwait();
            }

            var newStateDef = GroupDefinition.StateDefinitionFromName(newState);
            var hasData = !string.IsNullOrWhiteSpace(dataAsJson);

            if (newStateDef?.ChangingTo != null)
            {
                "Invoking 'ChangingTo' on new state definition".Log();
                await newStateDef.ChangingTo(cancelToken).SafeAwait();
            }

            if (hasData && newStateDef?.ChangingToWithData != null)
            {
                "Invoking 'ChangingToWithData' on new state definition".Log();
                await newStateDef.ChangingToWithData(dataAsJson, cancelToken).SafeAwait();
            }
        }

        /// <summary>
        /// Change to the new state
        /// Note: If overriding, make sure to call the base method!
        /// </summary>
        /// <param name="newState">The name of the new state to transition to</param>
        /// <param name="isNewState">Is this a new state (forward) or going back</param>
        /// <param name="useTransitions">Should use transitions</param>
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled</param>
        /// <returns>Task to await</returns>
#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected virtual async Task ChangeCurrentState(string newState, bool isNewState, bool useTransitions, CancellationToken cancelToken)
#pragma warning restore 1998
        {
            // ReSharper disable once SuspiciousTypeConversion.Global // NOT HELPFUL
            if (CurrentStateData is IIsAbleToBeBlocked isBlockable)
            {
                "Detaching event handlers for isblocked on current view model".Log();
                isBlockable.IsBlockedChanged -= IsBlockable_IsBlockedChanged;
            }

            var newStateDef = GroupDefinition.StateDefinitionFromName(newState); // States.SafeValue(newState);
            var newStateDataWrapper = newStateDef?.UntypedStateDataWrapper;

            if (newStateDataWrapper != null)
            {
                "Retrieving existing ViewModel for new state".Log();
                var stateData = Existing(newStateDataWrapper.StateDataType);
                if (stateData == null)
                {
                    // var newGen = States[newState] as IGenerateViewModel;
                    // if (newGen == null) return false;
                    "Generating ViewModel for new state".Log();
                    stateData = newStateDataWrapper.Generate();

                    "Registering dependencies".Log();
                    // ReSharper disable once SuspiciousTypeConversion.Global - data entities can implement both interfaces
                    (stateData as IRegisterDependencies)?.RegisterDependencies(DependencyContainer);

                    // ReSharper disable once SuspiciousTypeConversion.Global //NOT HELPFUL
                    if (stateData is IInitialise initData)
                    {
                        await initData.Initialise(cancelToken).SafeAwait();
                    }

                    await newStateDataWrapper.InvokeInitialise(stateData, cancelToken).SafeAwait();
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
            CurrentStateDefinition?.TransitionTo(StateValueTargets, GroupDefinition.DefaultValues);
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
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled</param>
        /// <returns>Task to be awaited</returns>
#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected virtual async Task ChangedToState(string oldState, string dataAsJson, bool isNewState, bool useTransitions, CancellationToken cancelToken)
#pragma warning restore 1998
        {
            var oldStateDef = GroupDefinition.StateDefinitionFromName(oldState);
            if (oldStateDef?.ChangedFrom != null)
            {
                "Invoking ChangedFrom on old state definition".Log();
                await oldStateDef.ChangedFrom(cancelToken).SafeAwait();
            }

            var oldStateDataWrapper = oldStateDef?.UntypedStateDataWrapper;
            if (oldStateDataWrapper != null)
            {
                "Invoking ChangedFrom on current state definition".Log();
                await oldStateDataWrapper.InvokeChangedFrom(CurrentStateData, cancelToken).SafeAwait();
            }

            var currentStateDef = CurrentStateDefinition;
            var currentStateDataWrapper = CurrentStateDataWrapper;

            var hasData = !string.IsNullOrWhiteSpace(dataAsJson);

            // ReSharper disable once SuspiciousTypeConversion.Global // NOT HELPFUL
            if (CurrentStateData is IChangedTo arrived)
            {
                "Invoking Arriving on new ViewModel".Log();
                await arrived.ChangedTo(cancelToken).SafeAwait();
            }

            // ReSharper disable once SuspiciousTypeConversion.Global // NOT HELPFUL
            if (hasData && CurrentStateData is IChangedToWithData arrivedWithData)
            {
                "Invoking Arriving on new ViewModel".Log();
                await arrivedWithData.ChangedToWithData(dataAsJson, cancelToken).SafeAwait();
            }

            if (currentStateDef?.ChangedTo != null)
            {
                $"State definition found, of type {currentStateDef.GetType().Name}, invoking ChangedTo method".Log();
                await currentStateDef.ChangedTo(cancelToken).SafeAwait();
                "ChangedTo completed".Log();
            }
            else
            {
                "No new state definition".Log();
            }

            if (hasData && currentStateDef?.ChangedToWithJsonData != null)
            {
                $"State definition found, of type {currentStateDef.GetType().Name}, invoking ChangedToWithJsonData method".Log();
                await currentStateDef.ChangedToWithJsonData(dataAsJson, cancelToken).SafeAwait();
                "ChangedToWithJsonData completed".Log();
            }
            else
            {
                "No new state definition".Log();
            }

            if (currentStateDataWrapper != null)
            {
                "Invoking ChangedTo on new state definition".Log();
                await currentStateDataWrapper.InvokeChangedTo(CurrentStateData, cancelToken).SafeAwait();

                if (hasData)
                {
                    await currentStateDataWrapper.InvokeChangedToWithData(CurrentStateData, dataAsJson, cancelToken).SafeAwait();
                }
            }

            // Raise event after state changed
            await NotifyStateChanged(CurrentStateName, isNewState, useTransitions, cancelToken);
        }

        /// <summary>
        /// Overridable method to raise StateChanged event
        /// </summary>
        /// <param name="newState">The name of the state being changed to</param>
        /// <param name="isNewState">Whether the new state is a new state or being returned to</param>
        /// <param name="useTransitions">Indicates whether to use transitions</param>
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled</param>
        /// <returns>Task to be awaited</returns>
#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected virtual async Task NotifyStateChanged(string newState, bool isNewState, bool useTransitions, CancellationToken cancelToken)
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
                        StateChanged?.Invoke(this, new StateEventArgs(CurrentStateName, useTransitions, isNewState, cancelToken));
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
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled</param>
        /// <returns>Whether the state change should be cancelled (true)</returns>
#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected virtual async Task<bool> NotifyStateChanging(string newState, bool isNewState, bool useTransitions, CancellationToken cancelToken)
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
                        var cancel = new StateCancelEventArgs(CurrentStateName, useTransitions, isNewState, cancelToken);
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
        /// Determines whether the supplied state definition is the default state
        /// </summary>
        /// <param name="stateDefinition">The state definition</param>
        /// <returns>True if this is the default state</returns>
        protected virtual bool IsDefaultState(IStateDefinition stateDefinition)
        {
            return IsDefaultState(stateDefinition?.StateName);
        }

        /// <summary>
        /// Determines whether the supplied state name is the default state
        /// </summary>
        /// <param name="stateName">The state name</param>
        /// <returns>True if this is the default state</returns>
        protected virtual bool IsDefaultState(string stateName)
        {
            return string.IsNullOrWhiteSpace(stateName);
        }

        /// <summary>
        /// Method for raising the event indicating that going to previous has changed
        /// </summary>
        protected virtual void OnGoToPreviousStateIsBlockedChanged()
        {
            GoToPreviousStateIsBlockedChanged.SafeRaise(this);
        }

        /// <summary>
        /// Retrieves any existing entity for a particular entity type
        /// </summary>
        /// <param name="stateDataType">The type to use to look up state data</param>
        /// <returns>The state data (or null)</returns>
        protected virtual INotifyPropertyChanged Existing(Type stateDataType) =>
            stateDataType == null ? null : StateDataCache.SafeValue(stateDataType);

        private static TStateGroupDefinition CachedOrNewGroupDefinitionByKey(string cacheKey)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return new TStateGroupDefinition();
            }

            var def = CachedGroupDefinitions.SafeValue(cacheKey);
            return def ?? CreateAndCacheGroupDefinition(cacheKey);
        }

        private static TStateGroupDefinition CreateAndCacheGroupDefinition(string cacheKey)
        {
            var def = new TStateGroupDefinition();
            CachedGroupDefinitions[cacheKey] = def;
            return def;
        }

        private async void UpdateStatesByTriggers()
        {
            // Don't change state if current state triggers are still active
            if (CurrentStateDefinition.AllTriggersActive())
            {
                return;
            }

            var firstActiveState = GroupDefinition.States.FirstOrDefault(x => x.Value.AllTriggersActive());
            // If there's no active state, then just return
            if (IsDefaultState(firstActiveState.Value))
            {
                return;
            }

            if (CurrentStateName == firstActiveState.Key)
            {
                return;
            }

            await ChangeToStateByName(firstActiveState.Key);
        }

        private void IsBlockable_IsBlockedChanged(object sender, EventArgs e)
        {
            OnGoToPreviousStateIsBlockedChanged();
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
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled</param>
        /// <returns>Indicates successful transition to the newState</returns>
        private async Task<bool> PerformStateChange(string newState, bool isNewState, bool useTransitions, string data, CancellationToken cancelToken)
        {
            var newCancellation = new CancellationTokenSource();
            var cancel = newCancellation.Token;

            // Cancel and set the new cancellation source
            // using lock so that there is no opportunity for another thread
            // to set the source between the previous one being cancelled and
            // a new one being set
            lock (CancellationLock)
            {
                // Set a new cancellation source
                var currentCancellation = StateTransitionCancellation;

                // If a cancellation token was passed in, make sure
                // it's linked to the internal cancellation source
                if (cancelToken != CancellationToken.None)
                {
                    cancelToken.Register(newCancellation.Cancel);
                }

                StateTransitionCancellation = newCancellation;

                // Cancel any existing state transition
                currentCancellation?.Cancel();
            }

            try
            {
                await StateTransitionSemaphore.WaitAsync(cancel);
            }
            catch (OperationCanceledException ex)
            {
                // Wait was cancelled, so we didn't acquire the lock. Just return whether we're in the target state or not
                ex.LogException();
                return IsCurrentState(newState);
            }

            try
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
                var success = await AboutToChangeFrom(newState, data, isNewState, useTransitions, cancel);
                if (!success)
                {
                    return false;
                }

                // Invoke changing methods - not cancellable but allows freeing up resources/event handlers etc
                "Invoking ChangingFrom before state change".Log();
                await ChangingFrom(newState, data, isNewState, useTransitions, cancel);

                // Perform the state change
                "Invoking ChangeCurrentState to perform state change".Log();
                await ChangeCurrentState(newState, isNewState, useTransitions, cancel);

                // Perform post-change methods
                "Invoking ChangedToState after state change".Log();
                await ChangedToState(current, data, isNewState, useTransitions, cancel);

                "ChangeTo completed".Log();
                return true;
            }
            catch (Exception ex)
            {
                ex.LogException();
                return IsCurrentState(newState);
            }
            finally
            {
                // Make sure the semaphore is released
                StateTransitionSemaphore.Release();
            }
        }

        private void Trigger_IsActiveChanged(object sender, EventArgs e)
        {
            UpdateStatesByTriggers();
        }
    }
}