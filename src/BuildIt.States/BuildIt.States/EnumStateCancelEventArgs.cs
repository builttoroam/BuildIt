namespace BuildIt.States
{
    /// <summary>
    /// Event args for cancelling a state change
    /// </summary>
    /// <typeparam name="TState">The type (enum) of the state</typeparam>
    public class EnumStateCancelEventArgs<TState> : StateCancelEventArgs
        where TState : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumStateCancelEventArgs{TState}"/> class.
        /// Constructs a new instance
        /// </summary>
        /// <param name="state">The typed state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <param name="isNewState">Whether this is a new state, or go back to previous</param>
        public EnumStateCancelEventArgs(TState state, bool useTransitions, bool isNewState)
        {
            EnumState = state;
            UseTransitions = useTransitions;
            IsNewState = isNewState;
        }

        /// <summary>
        /// Gets the state name (based on enum value)
        /// </summary>
        public override string StateName => EnumState + string.Empty;

        /// <summary>
        /// Gets the enum value of the state
        /// </summary>
        public TState EnumState { get; }
    }
}