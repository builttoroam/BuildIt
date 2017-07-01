using System;

namespace BuildIt.States.Interfaces
{
    public interface INotifyStateChanging
    {
        event EventHandler<StateCancelEventArgs> StateChanging;
    }
}