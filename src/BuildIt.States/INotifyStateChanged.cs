using System;
using System.Threading.Tasks;

namespace BuildIt.States
{
    public interface INotifyStateChanged<TState>
        where TState:struct 
    {
        event EventHandler<StateEventArgs<TState>> StateChanged;

        Task<bool> ChangeTo(TState newState, bool useTransition = true);
    }
}