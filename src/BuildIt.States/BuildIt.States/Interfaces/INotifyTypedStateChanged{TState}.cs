using System;
using BuildIt.States.Typed;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Raises typed state changed event
    /// </summary>
    /// <typeparam name="TState">The type (enum) of the state</typeparam>
    public interface INotifyTypedStateChanged<TState> : INotifyStateChanged
    {
        /// <summary>
        /// Typed state changed event
        /// </summary>
        event EventHandler<TypedStateEventArgs<TState>> TypedStateChanged;
    }
}