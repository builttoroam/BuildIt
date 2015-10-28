using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BuildIt.Lifecycle.States
{
    public interface ITransitionDefinition<TState> where TState : struct
    {
        Func<TState, CancelEventArgs, Task> LeavingState { get; }
        TState StartState { get; set; }

        Func<TState, Task> ArrivingState { get; }
        TState EndState { get; set; }

        Func<TState, Task> ArrivedState { get; }
    }
}