using System.Threading.Tasks;

namespace BuildIt.States.Interfaces.StateData
{
    /// <summary>
    /// Method on entity should be called when about to change from state
    /// </summary>
    public interface IAboutToChangeFrom
    {
        /// <summary>
        /// Method called when about to change from state
        /// </summary>
        /// <param name="cancel">Can cancel the state change</param>
        /// <returns>Task to await</returns>
        Task AboutToChangeFrom(CancelEventArgs cancel);
    }
}