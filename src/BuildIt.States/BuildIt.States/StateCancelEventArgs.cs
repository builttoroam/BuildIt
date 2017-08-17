using System.Threading;
using System.Threading.Tasks;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    /// <summary>
    /// Cancel event args for state changes
    /// </summary>
    public class StateCancelEventArgs : CancelEventArgs, IStateCancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateCancelEventArgs"/> class.
        /// Creates a new event args instance
        /// </summary>
        /// <param name="state">The new state</param>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <param name="isNewState">Whether this is a new state, or go back to previous state</param>
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled</param>
        public StateCancelEventArgs(string state, bool useTransitions, bool isNewState, CancellationToken cancelToken)
            : this(useTransitions, isNewState, cancelToken)
        {
            StateName = state;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateCancelEventArgs"/> class.
        /// Empty constructor to support subclassing
        /// </summary>
        /// <param name="useTransitions">Whether to use transitions</param>
        /// <param name="isNewState">Whether this is a new state, or go back to previous state</param>
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled</param>
        protected StateCancelEventArgs(bool useTransitions, bool isNewState, CancellationToken cancelToken)
        {
            UseTransitions = useTransitions;
            IsNewState = isNewState;
            CancelToken = cancelToken;
            if (CancelToken != CancellationToken.None)
            {
                CancelToken.Register(() =>
                {
                    if (CancelToken.IsCancellationRequested)
                    {
                        Cancel = true;
                    }
                });
            }
        }

        /// <summary>
        /// Gets the state name
        /// </summary>
        public virtual string StateName { get; }

        /// <summary>
        /// Gets a value indicating whether whether to use transtions
        /// </summary>
        public bool UseTransitions { get; }

        /// <summary>
        /// Gets a value indicating whether whether it's a new state
        /// </summary>
        public bool IsNewState { get; }

        /// <summary>
        /// Gets the cancellation token
        /// </summary>
        public CancellationToken CancelToken { get; }

        private EventDeferral Deferral { get; set; }

        /// <summary>
        /// Allows the event to be deferred until processing complete
        /// </summary>
        /// <returns>deferral entity</returns>
        public EventDeferral Defer()
        {
            var semaphore = new SemaphoreSlim(1);
            Deferral = new EventDeferral(semaphore);
            return Deferral;
        }

        /// <summary>
        /// Allows the event to be completed
        /// </summary>
        /// <returns>Task to await</returns>
        public async Task CompleteEvent()
        {
            var deferral = Deferral?.Deferral;
            if (deferral != null)
            {
                await deferral.WaitAsync();
            }
        }
    }
}