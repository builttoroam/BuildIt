using System;

namespace BuildIt.Lifecycle.States
{
    public interface INotifyStateChanged<TState>
        where TState:struct 
    {
        event EventHandler<StateEventArgs<TState>> StateChanged;
    }
}