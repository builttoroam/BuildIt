using System;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Entity that raises a typed state changing event
    /// </summary>
    /// <typeparam name="TState">The type (enum) of the state</typeparam>
    public interface INotifyEnumStateChanging<TState> : INotifyStateChanging
        where TState : struct
    {
        /// <summary>
        /// Typed state changing event
        /// </summary>
        event EventHandler<EnumStateCancelEventArgs<TState>> EnumStateChanging;
    }
}