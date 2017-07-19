using System;
using System.Reflection;

namespace BuildIt.States
{
    /// <summary>
    /// A typed state group
    /// </summary>
    /// <typeparam name="TState">The type (enum) fo the state group</typeparam>
    public class EnumStateGroup<TState> : TypedStateGroup<TState>
        where TState : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumStateGroup{TState}"/> class.
        /// </summary>
        public EnumStateGroup()
            : this(new EnumStateGroupDefinition<TState>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumStateGroup{TState}"/> class.
        /// </summary>
        /// <param name="groupDefinition">An existing group definition</param>
        public EnumStateGroup(EnumStateGroupDefinition<TState> groupDefinition)
            : base(groupDefinition)
        {
        }

        /// <summary>
        /// Gets or sets the current state name
        /// </summary>
        public override string CurrentStateName
        {
            get => (!default(TState).Equals(CurrentState)) ? CurrentState + string.Empty : null;
            protected set => CurrentState = value.EnumParse<TState>();
        }
    }
}