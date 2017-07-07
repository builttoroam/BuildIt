using System;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Entity that raises a state changing event
    /// </summary>
    public interface INotifyStateChanging
    {
        /// <summary>
        /// State changing event
        /// </summary>
        event EventHandler<StateCancelEventArgs> StateChanging;
    }
}