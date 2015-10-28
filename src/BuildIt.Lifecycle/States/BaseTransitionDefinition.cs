using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BuildIt.Lifecycle.States
{
    public class BaseTransitionDefinition<TState> :
        ITransitionDefinition<TState> where TState : struct
    {
        public Func<TState, CancelEventArgs, Task> LeavingState { get; set; }
        public TState StartState { get; set; }

        public Func<TState, Task> ArrivingState { get; set; }
        public TState EndState { get; set; }

        public Func<TState, Task> ArrivedState { get; set; }
    }
}