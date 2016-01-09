using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BuildIt.States
{
    public interface IStateDefinition<TState> where TState : struct
    {
        TState State { get; }

        Func<CancelEventArgs, Task> AboutToChangeFrom { get; set; }
        Func<Task> ChangingFrom { get; set; }
        Func<Task> ChangedTo { get; set; }
        Func<string,Task> ChangedToWithJsonData { get; set; }

        void TransitionTo(IDictionary<Tuple<object, string>, IDefaultValue> defaultValues);

    }
}