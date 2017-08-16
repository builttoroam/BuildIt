using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces.StateData
{
    /// <summary>
    /// Method on entity that should be called immediately after item is constructed
    /// </summary>
    public interface IInitialise
    {
        /// <summary>
        /// Method called after construction
        /// </summary>
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled</param>
        /// <returns>Task to await</returns>
        Task Initialise(CancellationToken cancelToken);
    }
}