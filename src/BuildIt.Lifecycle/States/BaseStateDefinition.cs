using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BuildIt.Lifecycle.States
{
    public class BaseStateDefinition<TState> : IStateDefinition<TState>
        where TState : struct
    {
        public TState State { get; set; }

        public Func<CancelEventArgs, Task> AboutToChangeFrom { get; set; }
        public Func<Task> ChangingFrom { get; set; }
        public Func<Task> ChangedTo { get; set; }

    }
}