using System.Threading.Tasks;

namespace BuildIt.States.Interfaces.StateData
{
    /// <summary>
    /// Method on entity should be called when changing from state
    /// </summary>
    public interface IChangingFrom
    {
        /// <summary>
        /// Method called when changing from state
        /// </summary>
        /// <returns></returns>
        Task ChangingFrom();
    }
}