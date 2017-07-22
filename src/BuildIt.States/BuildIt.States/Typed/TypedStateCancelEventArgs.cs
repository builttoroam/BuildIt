namespace BuildIt.States.Typed
{
    /// <summary>
    /// Event args for cancelling a state change
    /// </summary>
    /// <typeparam name="TState">The type of the state</typeparam>
    public class TypedStateCancelEventArgs<TState> : StateCancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypedStateCancelEventArgs{TState}"/> class.
        /// Constructs a new instance
        /// </summary>
        /// <param name="state">The typed state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <param name="isNewState">Whether this is a new state, or go back to previous</param>
        public TypedStateCancelEventArgs(TState state, bool useTransitions, bool isNewState)
        {
            TypedState = state;
            UseTransitions = useTransitions;
            IsNewState = isNewState;
        }

        /// <summary>
        /// Gets the state name
        /// </summary>
        public override string StateName => TypedState + string.Empty;

        /// <summary>
        /// Gets the enum value of the state
        /// </summary>
        public TState TypedState { get; }
    }
}