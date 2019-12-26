using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Event args for when state changes.
    /// </summary>
    public interface IStateEventArgs
    {
        /// <summary>
        /// Gets the state name.
        /// </summary>
        string StateName { get; }

        /// <summary>
        /// Gets a value indicating whether whether to use transitions.
        /// </summary>
        bool UseTransitions { get; }

        /// <summary>
        /// Gets a value indicating whether whether it's a new state.
        /// </summary>
        bool IsNewState { get; }

        /// <summary>
        /// Gets the cancellation token.
        /// </summary>
        CancellationToken CancelToken { get; }

        /// <summary>
        /// Allows the event to be deferred until processing complete.
        /// </summary>
        /// <returns>deferral entity.</returns>
        EventDeferral Defer();

        /// <summary>
        /// Allows the event to be completed.
        /// </summary>
        /// <returns>Task to await.</returns>
        Task CompleteEvent();
    }
}