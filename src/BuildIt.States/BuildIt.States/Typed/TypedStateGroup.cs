using System;
using System.Threading;
using System.Threading.Tasks;
using BuildIt.States.Interfaces;

namespace BuildIt.States.Typed
{
    /// <summary>
    /// A typed state group
    /// </summary>
    /// <typeparam name="TState">The type (enum) fo the state group</typeparam>
    /// <typeparam name="TStateDefinition">The type of the state definition</typeparam>
    /// <typeparam name="TStateGroupDefinition">The type of the group definition</typeparam>
    public abstract class TypedStateGroup<TState, TStateDefinition, TStateGroupDefinition>
        : BaseStateGroup<TStateDefinition, TStateGroupDefinition>, ITypedStateGroup<TState, TStateDefinition, TStateGroupDefinition>
        where TStateDefinition : class, ITypedStateDefinition<TState>, new()
        where TStateGroupDefinition : class, ITypedStateGroupDefinition<TState, TStateDefinition>, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypedStateGroup{TState, TStateDefinition, TStateGroupDefinition}"/> class.
        /// </summary>
        /// <param name="cacheKey">The cacheKey for the state definition</param>
        protected TypedStateGroup(string cacheKey = null)
            : base(cacheKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedStateGroup{TState, TStateDefinition, TStateGroupDefinition}"/> class.
        /// </summary>
        /// <param name="groupDefinition">An existing group definition</param>
        protected TypedStateGroup(TStateGroupDefinition groupDefinition)
            : base(groupDefinition)
        {
        }

        /// <summary>
        /// Event indicating that the current state for this group is about to change
        /// </summary>
        public event EventHandler<ITypedStateCancelEventArgs<TState>> TypedStateAboutToChange;

        /// <summary>
        /// Event indicating that the current state for this group is changing
        /// </summary>
        public event EventHandler<ITypedStateEventArgs<TState>> TypedStateChanging;

        /// <summary>
        /// Event indicating that the current state for this group has changed
        /// </summary>
        public event EventHandler<ITypedStateEventArgs<TState>> TypedStateChanged;

        /// <summary>
        /// Event indicating that the state change has completed
        /// </summary>
        public event EventHandler<ITypedStateEventArgs<TState>> TypedStateChangeComplete;

        /// <summary>
        /// Gets or sets the current typed state
        /// </summary>
        public TState CurrentState { get; protected set; }

        /// <summary>
        /// Sets the current state name
        /// </summary>
        public override string CurrentStateName
        {
            protected set
            {
                base.CurrentStateName = value;
                CurrentState = CurrentTypedStateDefinition.State;
            }
        }

        /// <summary>
        /// Change to typed state, with data
        /// </summary>
        /// <typeparam name="TData">The type of data to pass to new state</typeparam>
        /// <param name="findState">The new state</param>
        /// <param name="data">The data to pass to the new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Success if change is completed</returns>
        public Task<bool> ChangeToStateWithData<TData>(TState findState, TData data, bool useTransitions = true)
        {
            return ChangeToStateWithData(findState, data, useTransitions, CancellationToken.None);
        }

        /// <summary>
        /// Change to typed state
        /// </summary>
        /// <param name="findState">The new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Success if change is completed</returns>
        public Task<bool> ChangeToState(TState findState, bool useTransitions = true)
        {
            return ChangeToState(findState, useTransitions, CancellationToken.None);
        }

        /// <summary>
        /// Changes back to a typed state
        /// </summary>
        /// <param name="findState">The state to change back to</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Success if change is completed</returns>
        public Task<bool> ChangeBackToState(TState findState, bool useTransitions = true)
        {
            return ChangeBackToState(findState, useTransitions, CancellationToken.None);
        }

        /// <summary>
        /// Change to typed state, with data
        /// </summary>
        /// <typeparam name="TData">The type of data to pass to new state</typeparam>
        /// <param name="findState">The new state</param>
        /// <param name="data">The data to pass to the new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <param name="cancel">Cancellation token allowing change to be cancelled</param>
        /// <returns>Success if change is completed</returns>
        public async Task<bool> ChangeToStateWithData<TData>(TState findState, TData data, bool useTransitions, CancellationToken cancel)
        {
            return await ChangeToStateByNameWithData(findState + string.Empty, data, useTransitions, cancel);
        }

        /// <summary>
        /// Change to typed state
        /// </summary>
        /// <param name="findState">The new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <param name="cancel">Cancellation token allowing change to be cancelled</param>
        /// <returns>Success if change is completed</returns>
        public async Task<bool> ChangeToState(TState findState, bool useTransitions, CancellationToken cancel)
        {
            return await ChangeToStateByName(findState + string.Empty, useTransitions, cancel);
        }

        /// <summary>
        /// Changes back to a typed state
        /// </summary>
        /// <param name="findState">The state to change back to</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <param name="cancel">Cancellation token allowing change to be cancelled</param>
        /// <returns>Success if change is completed</returns>
        public async Task<bool> ChangeBackToState(TState findState, bool useTransitions, CancellationToken cancel)
        {
            if (TrackHistory == false)
            {
                throw new Exception("History tracking not enabled");
            }

            return await ChangeBackToStateByName(findState + string.Empty, useTransitions, cancel);
        }

        /// <summary>
        /// Overrides method to raise the StateAboutToChange event
        /// </summary>
        /// <param name="newState">The new state to transition to</param>
        /// <param name="isNewState">Whether this will be a new state or going to previous</param>
        /// <param name="useTransitions">Whether to use transitions or not</param>
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled</param>
        /// <returns>Whether the state change should be cancelled (true)</returns>
#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected override async Task<bool> NotifyStateAboutToChange(string newState, bool isNewState, bool useTransitions, CancellationToken cancelToken)
#pragma warning restore 1998
        {
            var shouldCancel = false;
            try
            {
                shouldCancel = await base.NotifyStateAboutToChange(newState, isNewState, useTransitions, cancelToken);
                if (shouldCancel)
                {
                    return true;
                }

                "Invoking Typed StateAboutToChange event".Log();
                var state = TypedGroupDefinition.TypedStateDefinitionFromName(newState);
                if (state == null)
                {
                    return false;
                }

                var cancelArgs = new TypedStateCancelEventArgs<TState>(state.State, useTransitions, isNewState, cancelToken);
                await RaiseTypedStateEvent(TypedStateAboutToChange, cancelArgs);
                shouldCancel = cancelArgs.Cancel;
                "Typed StateAboutToChange event completed".Log();
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
        /// Overrides method to raise StateChanging event
        /// </summary>
        /// <param name="newState">The name of the state being changed to</param>
        /// <param name="isNewState">Whether the new state is a new state or being returned to</param>
        /// <param name="useTransitions">Indicates whether to use transitions</param>
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled</param>
        /// <returns>Task to be awaited</returns>
#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected override async Task NotifyStateChanging(string newState, bool isNewState, bool useTransitions, CancellationToken cancelToken)
#pragma warning restore 1998
        {
            try
            {
                await base.NotifyStateChanging(newState, isNewState, useTransitions, cancelToken);

                "Invoking Typed StateChanging event".Log();
                var state = TypedGroupDefinition.TypedStateDefinitionFromName(newState);
                if (state == null)
                {
                    return;
                }

                var args = new TypedStateEventArgs<TState>(state.State, useTransitions, isNewState, cancelToken);
                await RaiseTypedStateEvent(TypedStateChanging, args);
                "Typed StateChanging event completed".Log();
            }
            catch (Exception ex)
            {
                ex.LogException();
                // Ignore any errors caused by the event being raised, as
                // the state change has still occurred
            }
        }

        /// <summary>
        /// Overrides method to raise StateChanged event
        /// </summary>
        /// <param name="newState">The name of the state being changed to</param>
        /// <param name="isNewState">Whether the new state is a new state or being returned to</param>
        /// <param name="useTransitions">Indicates whether to use transitions</param>
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled</param>
        /// <returns>Task to be awaited</returns>
#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected override async Task NotifyStateChanged(string newState, bool isNewState, bool useTransitions, CancellationToken cancelToken)
#pragma warning restore 1998
        {
            try
            {
                await base.NotifyStateChanged(newState, isNewState, useTransitions, cancelToken);

                "Invoking Typed StateChanged event".Log();
                var state = TypedGroupDefinition.TypedStateDefinitionFromName(newState);
                if (state == null)
                {
                    return;
                }

                var args = new TypedStateEventArgs<TState>(state.State, useTransitions, isNewState, cancelToken);
                await RaiseTypedStateEvent(TypedStateChanged, args);
                "Typed StateChanged event completed".Log();
            }
            catch (Exception ex)
            {
                ex.LogException();
                // Ignore any errors caused by the event being raised, as
                // the state change has still occurred
            }
        }

        /// <summary>
        /// Overrides method to raise StateChangeComplete event
        /// </summary>
        /// <param name="newState">The name of the state being changed to</param>
        /// <param name="isNewState">Whether the new state is a new state or being returned to</param>
        /// <param name="useTransitions">Indicates whether to use transitions</param>
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled</param>
        /// <returns>Task to be awaited</returns>
#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected override async Task NotifyStateChangeComplete(string newState, bool isNewState, bool useTransitions, CancellationToken cancelToken)
#pragma warning restore 1998
        {
            try
            {
                await base.NotifyStateChangeComplete(newState, isNewState, useTransitions, cancelToken);

                "Invoking Typed StateChangeComplete event".Log();
                var state = TypedGroupDefinition.TypedStateDefinitionFromName(newState);
                if (state == null)
                {
                    return;
                }

                var args = new TypedStateEventArgs<TState>(state.State, useTransitions, isNewState, cancelToken);
                await RaiseTypedStateEvent(TypedStateChangeComplete, args);
                "Typed StateChangeComplete event completed".Log();
            }
            catch (Exception ex)
            {
                ex.LogException();
                // Ignore any errors caused by the event being raised, as
                // the state change has still occurred
            }
        }

        private async Task RaiseTypedStateEvent<TStateEventArgs>(EventHandler<TStateEventArgs> eventToRaise, TStateEventArgs args)
            where TStateEventArgs : ITypedStateEventArgs<TState>
        {
            try
            {
                var localReferenceToEvent = eventToRaise;
                if (localReferenceToEvent != null)
                {
                    "Invoking event (before UI context check)".Log();
                    await UIContext.RunAsync(async () =>
                    {
                        "Raising event".Log();
                        localReferenceToEvent.Invoke(this, args);
                        "Raising event completed - waiting for lock to be release".Log();
                        await args.CompleteEvent();
                    });
                    "event completed (after UI context check)".Log();
                }
                else
                {
                    "Nothing listening to event".Log();
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
                // Ignore any errors caused by the event being raised, as
                // the state change has still occurred
            }
        }
    }
}