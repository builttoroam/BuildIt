using System;

namespace BuildIt.States.Interfaces
{
    public interface INotifyEnumStateChanged<TState>:INotifyStateChanged
        where TState:struct 
    {
        event EventHandler<EnumStateEventArgs<TState>> EnumStateChanged;
    }
}