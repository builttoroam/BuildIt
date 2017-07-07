using System;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Raises typed state changed event
    /// </summary>
    /// <typeparam name="TState">The type (enum) of the state</typeparam>
    public interface INotifyEnumStateChanged<TState>: INotifyStateChanged
        where TState : struct
    {
        /// <summary>
        /// Typed state changed event
        /// </summary>
        event EventHandler<EnumStateEventArgs<TState>> EnumStateChanged;
    }
}