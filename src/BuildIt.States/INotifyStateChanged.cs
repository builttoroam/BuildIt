using System;

namespace BuildIt.States
{
    public interface INotifyStateChanged<TState>
        where TState:struct 
    {
        event EventHandler<StateEventArgs<TState>> StateChanged;
    }
}