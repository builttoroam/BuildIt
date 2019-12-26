using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces.StateData
{
    /// <summary>
    /// Method on entity should be called when changed to state.
    /// </summary>
    public interface IChangedTo
    {
        /// <summary>
        /// Method called when changed to state.
        /// </summary>
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled.</param>
        /// <returns>Task to await.</returns>
        Task ChangedTo(CancellationToken cancelToken);
    }
}