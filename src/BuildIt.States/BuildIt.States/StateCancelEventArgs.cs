namespace BuildIt.States
{
    /// <summary>
    /// Cancel event args for state changes
    /// </summary>
    public class StateCancelEventArgs : CancelEventArgs, IStateEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateCancelEventArgs"/> class.
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
        /// Initializes a new instance of the <see cref="StateCancelEventArgs"/> class.
        /// Empty constructor to support subclassing
        /// </summary>
        protected StateCancelEventArgs()
        {
        }

        /// <summary>
        /// Gets the state name
        /// </summary>
        public virtual string StateName { get; }

        /// <summary>
        /// Gets or sets a value indicating whether whether to use transtions
        /// </summary>
        public bool UseTransitions { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether whether it's a new state
        /// </summary>
        public bool IsNewState { get; protected set; }
    }
}