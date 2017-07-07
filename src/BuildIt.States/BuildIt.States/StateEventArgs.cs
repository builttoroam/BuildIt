using System;

namespace BuildIt.States
{
    /// <summary>
    /// Event args for state change and changing events
    /// </summary>
    public class StateEventArgs : EventArgs, IStateEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateEventArgs"/> class.
        /// Constructs new instance
        /// </summary>
        /// <param name="state">The state name</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <param name="isNewState">Whether this is a new state or going back to existing</param>
        public StateEventArgs(string state, bool useTransitions, bool isNewState)
        {
            StateName = state;
            UseTransitions = useTransitions;
            IsNewState = isNewState;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateEventArgs"/> class.
        /// Empty constructor to support subclassing
        /// </summary>
        protected StateEventArgs()
        {
        }

        /// <summary>
        /// Gets the name of the state
        /// </summary>
        public virtual string StateName { get; }

        /// <summary>
        /// Gets or sets a value indicating whether whether to use transitions
        /// </summary>
        public bool UseTransitions { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether whether it's a new state or going to a previous state
        /// </summary>
        public bool IsNewState { get; protected set; }
    }
}