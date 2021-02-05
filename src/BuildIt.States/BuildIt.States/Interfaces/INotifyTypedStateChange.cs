using System;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Entity that raises a typed state changing event.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    public interface INotifyTypedStateChange<TState> : INotifyStateChange
    {
        /// <summary>
        /// Typed Event indicating that the current state for this group is about to change
        /// </summary>
        event EventHandler<ITypedStateCancelEventArgs<TState>> TypedStateAboutToChange;

        /// <summary>
        /// Typed Event indicating that the current state for this group is changing
        /// </summary>
        event EventHandler<ITypedStateEventArgs<TState>> TypedStateChanging;

        /// <summary>
        /// Typed Event indicating that the current state for this group has changed
        /// </summary>
        event EventHandler<ITypedStateEventArgs<TState>> TypedStateChanged;

        /// <summary>
        /// Typed Event indicating that the state change has completed
        /// </summary>
        event EventHandler<ITypedStateEventArgs<TState>> TypedStateChangeComplete;
    }
}