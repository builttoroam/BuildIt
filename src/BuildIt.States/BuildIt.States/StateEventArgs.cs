using System;

namespace BuildIt.States
{
    /// <summary>
    /// Event args for state change and changing events
    /// </summary>
    public class StateEventArgs : EventArgs, IStateEventArgs
    {
        /// <summary>
        /// Empty constructor to support subclassing
        /// </summary>
        protected StateEventArgs()
        {

        }

        /// <summary>
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
        public virtual string StateName { get; }
        public bool UseTransitions { get; protected set; }
        public bool IsNewState { get; protected set; }

       
    }
}