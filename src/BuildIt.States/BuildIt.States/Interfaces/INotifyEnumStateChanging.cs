using System;

namespace BuildIt.States.Interfaces
{
    public interface INotifyEnumStateChanging<TState>: INotifyStateChanging
        where TState : struct
    {
        event EventHandler<EnumStateCancelEventArgs<TState>> EnumStateChanging;
    }
}