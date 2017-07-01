using System;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces
{
    public interface INotifyStateChanged
    {
        event EventHandler<StateEventArgs> StateChanged;
    }
}