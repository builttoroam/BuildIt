using System;
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
        /// Typed state changed event
        /// </summary>
        public event EventHandler<TypedStateEventArgs<TState>> TypedStateChanged;

        /// <summary>
        /// Typed state changing event
        /// </summary>
        public event EventHandler<TypedStateCancelEventArgs<TState>> TypedStateChanging;

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
        public async Task<bool> ChangeToStateWithData<TData>(TState findState, TData data, bool useTransitions = true)
        {
            return await ChangeToStateByNameWithData(findState + string.Empty, data, useTransitions);
        }

        /// <summary>
        /// Change to typed state
        /// </summary>
        /// <param name="findState">The new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Success if change is completed</returns>
        public async Task<bool> ChangeToState(TState findState, bool useTransitions = true)
        {
            return await ChangeToStateByName(findState + string.Empty, useTransitions);
        }

        /// <summary>
        /// Changes back to a typed state
        /// </summary>
        /// <param name="findState">The state to change back to</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <returns>Success if change is completed</returns>
        public async Task<bool> ChangeBackToState(TState findState, bool useTransitions = true)
        {
            if (TrackHistory == false)
            {
                throw new Exception("History tracking not enabled");
            }

            return await ChangeBackToStateByName(findState + string.Empty, useTransitions);
        }

        /// <summary>
        /// Overridable method to raise StateChanged event
        /// </summary>
        /// <param name="newState">The name of the state being changed to</param>
        /// <param name="isNewState">Whether the new state is a new state or being returned to</param>
        /// <param name="useTransitions">Indicates whether to use transitions</param>
        /// <returns>Task to be awaited</returns>
#pragma warning disable 1998 // Returns a Task so that overrides can do async work
        protected override async Task NotifyStateChanged(string newState, bool isNewState, bool useTransitions)
#pragma warning restore 1998
        {
            try
            {
                await base.NotifyStateChanged(newState, isNewState, useTransitions);

                if (TypedStateChanged != null)
                {
                    "Invoking TypedStateChanged event (before UI context check)".Log();
                    await UIContext.RunAsync(() =>
                    {
                        "Raising TypedStateChanged event".Log();
                        TypedStateChanged?.Invoke(this, new TypedStateEventArgs<TState>(CurrentState, useTransitions, isNewState));
                        "Raising TypedStateChanged event completed".Log();
                    });
                    "TypedStateChanged event completed (after UI context check)".Log();
                }
                else
                {
                    "Nothing listening to TypedStateChanged".Log();
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
        protected override async Task<bool> NotifyStateChanging(string newState, bool isNewState, bool useTransitions)
#pragma warning restore 1998
        {
            var shouldCancel = false;
            try
            {
                var statecancel = await base.NotifyStateChanging(newState, isNewState, useTransitions);
                if (statecancel)
                {
                    return true;
                }

                if (TypedStateChanging != null)
                {
                    "Invoking TypedStateChanging event (before UI context check)".Log();
                    await UIContext.RunAsync(() =>
                    {
                        var cancel = new TypedStateCancelEventArgs<TState>(CurrentState, useTransitions, isNewState);
                        "Raising TypedStateChanging event".Log();
                        TypedStateChanging?.Invoke(this, cancel);
                        "Raising TypedStateChanging event completed".Log();
                        shouldCancel = cancel.Cancel;
                    });
                    "TypedStateChanging event completed (after UI context check)".Log();
                }
                else
                {
                    "Nothing listening to TypedStateChanging".Log();
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
    }
}