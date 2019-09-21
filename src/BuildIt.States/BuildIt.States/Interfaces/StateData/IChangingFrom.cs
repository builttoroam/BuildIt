using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces.StateData
{
    /// <summary>
    /// Method on entity should be called when changing from state.
    /// </summary>
    public interface IChangingFrom
    {
        /// <summary>
        /// Method called when changing from state.
        /// </summary>
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled.</param>
        /// <returns>Task to be awaited.</returns>
        Task ChangingFrom(CancellationToken cancelToken);
    }
}