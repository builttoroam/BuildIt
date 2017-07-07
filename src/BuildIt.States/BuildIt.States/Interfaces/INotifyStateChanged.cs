using System;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Entity that raises a state changed event
    /// </summary>
    public interface INotifyStateChanged
    {
        /// <summary>
        /// State changed event
        /// </summary>
        event EventHandler<StateEventArgs> StateChanged;
    }
}