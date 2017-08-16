using System;
using System.Threading;

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
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled</param>
        public StateEventArgs(string state, bool useTransitions, bool isNewState, CancellationToken cancelToken)
            : this(useTransitions, isNewState, cancelToken)
        {
            StateName = state;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateEventArgs"/> class.
        /// Empty constructor to support subclassing
        /// </summary>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <param name="isNewState">Whether this is a new state, or go back to previous state</param>
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled</param>
        protected StateEventArgs(bool useTransitions, bool isNewState, CancellationToken cancelToken)
        {
            UseTransitions = useTransitions;
            IsNewState = isNewState;
            CancelToken = cancelToken;
        }

        /// <summary>
        /// Gets the name of the state
        /// </summary>
        public virtual string StateName { get; }

        /// <summary>
        /// Gets a value indicating whether whether to use transitions
        /// </summary>
        public bool UseTransitions { get; }

        /// <summary>
        /// Gets a value indicating whether whether it's a new state or going to a previous state
        /// </summary>
        public bool IsNewState { get; }

        /// <summary>
        /// Gets the cancellation token
        /// </summary>
        public CancellationToken CancelToken { get; }
    }
}