namespace BuildIt.States
{
    /// <summary>
    /// Event args for a state change
    /// </summary>
    /// <typeparam name="TState">The type (enum) fo the state</typeparam>
    public class TypedStateEventArgs<TState> : StateEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypedStateEventArgs{TState}"/> class.
        /// Constructs new instance
        /// </summary>
        /// <param name="state">The typed state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <param name="isNewState">Whether ist a new state, or go back to previous</param>
        public TypedStateEventArgs(TState state, bool useTransitions, bool isNewState)
        {
            TypedState = state;
            UseTransitions = useTransitions;
            IsNewState = isNewState;
        }

        /// <summary>
        /// Gets state name
        /// </summary>
        public override string StateName => TypedState + string.Empty;

        /// <summary>
        /// Gets typed state
        /// </summary>
        public TState TypedState { get;  }
    }
}