using System;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Entity that raises a state changing event.
    /// </summary>
    public interface INotifyStateChange
    {
        /// <summary>
        /// Event indicating that the current state for this group is about to change
        /// </summary>
        event EventHandler<IStateCancelEventArgs> StateAboutToChange;

        /// <summary>
        /// Event indicating that the current state for this group is changing
        /// </summary>
        event EventHandler<IStateEventArgs> StateChanging;

        /// <summary>
        /// Event indicating that the current state for this group has changed
        /// </summary>
        event EventHandler<IStateEventArgs> StateChanged;

        /// <summary>
        /// Event indicating that the state change has completed
        /// </summary>
        event EventHandler<IStateEventArgs> StateChangeComplete;
    }
}