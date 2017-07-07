namespace BuildIt.States
{
    /// <summary>
    /// Cancel event args for state changes
    /// </summary>
    public class StateCancelEventArgs : CancelEventArgs, IStateEventArgs
    {
        /// <summary>
        /// Empty constructor to support subclassing
        /// </summary>
        protected StateCancelEventArgs()
        {

        }

        /// <summary>
        /// Creates a new event args instance
        /// </summary>
        /// <param name="state">The new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <param name="isNewState">Whether this is a new state, or go back to previous state</param>
        public StateCancelEventArgs(string state, bool useTransitions, bool isNewState)
        {
            StateName = state;
            UseTransitions = useTransitions;
            IsNewState = isNewState;
        }

        /// <summary>
        /// The state name
        /// </summary>
        public virtual string StateName { get; }

/// <summary>
/// Whether to use transtions
/// </summary>
        public bool UseTransitions { get; protected set; }

        /// <summary>
        /// Whether it's a new state
        /// </summary>
        public bool IsNewState { get; protected set; }

    }
}