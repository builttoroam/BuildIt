using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces.StateData
{
    /// <summary>
    /// Method on entity should be called when changed to state, with data
    /// </summary>
    public interface IChangedToWithData
    {
        /// <summary>
        /// Method called when changed to state, with data
        /// </summary>
        /// <param name="dataAsJson">The data passed into the state</param>
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled</param>
        /// <returns>Task to be awaited</returns>
        Task ChangedToWithData(string dataAsJson, CancellationToken cancelToken);
    }
}