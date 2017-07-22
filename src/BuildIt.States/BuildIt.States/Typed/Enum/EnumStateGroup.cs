namespace BuildIt.States.Typed.Enum
{
    /// <summary>
    /// A typed state group
    /// </summary>
    /// <typeparam name="TState">The type (enum) fo the state group</typeparam>
    public class EnumStateGroup<TState>
        : TypedStateGroup<TState, EnumStateDefinition<TState>, EnumStateGroupDefinition<TState>>
        where TState : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumStateGroup{TState}"/> class.
        /// </summary>
        /// <param name="cacheKey">The cacheKey for the state definition</param>
        public EnumStateGroup(string cacheKey = null)
            : base(cacheKey)
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
    }
}