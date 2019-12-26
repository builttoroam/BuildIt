using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces.StateData
{
    /// <summary>
    /// Method on entity that should be called to initialise it after item has been created.
    /// </summary>
    /// <typeparam name="TData">Data to pass into the initialize method.</typeparam>
    public interface IInitialiseWithData<TData>
    {
        /// <summary>
        /// Method to initialise state data object with data.
        /// </summary>
        /// <param name="data">The data to pass in.</param>
        /// <param name="cancelToken">Cancellation token allowing change to be cancelled.</param>
        /// <returns>Task to await.</returns>
        Task InitialiseWithData(TData data, CancellationToken cancelToken);
    }
}