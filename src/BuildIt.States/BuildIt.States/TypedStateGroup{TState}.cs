using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    /// <summary>
    /// A typed state group
    /// </summary>
    /// <typeparam name="TState">The type (enum) fo the state group</typeparam>
    public class TypedStateGroup<TState> : BaseStateGroup, ITypedStateGroup<TState>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypedStateGroup{TState}"/> class.
        /// </summary>
        public TypedStateGroup()
            : this(new TypedStateGroupDefinition<TState>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedStateGroup{TState}"/> class.
        /// </summary>
        /// <param name="groupDefinition">An existing group definition</param>
        public TypedStateGroup(TypedStateGroupDefinition<TState> groupDefinition)
            : base(groupDefinition)
        {
        }

#pragma warning disable CS0067 // See TODO
        // TODO: Raise events at correct point when changing state

        /// <summary>
        /// Typed state changed event
        /// </summary>
        public event EventHandler<TypedStateEventArgs<TState>> TypedStateChanged;

        /// <summary>
        /// Typed state changing event
        /// </summary>
        public event EventHandler<TypedStateCancelEventArgs<TState>> TypedStateChanging;
#pragma warning restore CS0067

        /// <summary>
        /// Gets the state group definition (including the states that make up the group)
        /// </summary>
        public ITypedStateGroupDefinition<TState> TypedGroupDefinition => GroupDefinition as ITypedStateGroupDefinition<TState>;

        /// <summary>
        /// Gets or sets the current typed state
        /// </summary>
        public TState CurrentState { get; protected set; }

        /// <summary>
        /// Gets or sets the current state name
        /// </summary>
        public override string CurrentStateName
        {
            get => (!(default(TState)?.Equals(CurrentState) ?? false)) ? CurrentState + string.Empty : null;
            protected set => CurrentState = TypedGroupDefinition.FromString(value);
        }

        /// <summary>
        /// Gets returns the state definition for the current state
        /// </summary>
        public ITypedStateDefinition<TState> CurrentTypedStateDefinition
            => GroupDefinition.States.SafeValue(CurrentStateName) as ITypedStateDefinition<TState>;

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
    }
}