namespace BuildIt.States
{
    /// <summary>
    /// Event args for a state change
    /// </summary>
    /// <typeparam name="TState">The type (enum) fo the state</typeparam>
    public class EnumStateEventArgs<TState> : StateEventArgs
        where TState:struct
    {
        /// <summary>
        /// Constructs new instance
        /// </summary>
        /// <param name="state">The typed state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <param name="isNewState">Whether ist a new state, or go back to previous</param>
        public EnumStateEventArgs(TState state, bool useTransitions, bool isNewState)
        {
            EnumState = state;
            UseTransitions = useTransitions;
            IsNewState = isNewState;
        }

        /// <summary>
        /// State name
        /// </summary>
        public override string StateName => EnumState + "";

        /// <summary>
        /// Typed state 
        /// </summary>
        public TState EnumState { get;  }

    }
}