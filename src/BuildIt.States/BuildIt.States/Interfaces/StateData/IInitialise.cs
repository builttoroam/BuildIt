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
        /// <returns>Task to await</returns>
        Task Initialise();
    }
}