using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces
{
    public interface IStateDefinition
    {
        string StateName { get; }

        IList<IStateTrigger> Triggers { get; }

        Func<CancelEventArgs, Task> AboutToChangeFrom { get; set; }
        Func<Task> ChangingFrom { get; set; }
        Func<Task> ChangedTo { get; set; }
        Func<string, Task> ChangedToWithJsonData { get; set; }

        IList<IStateValue> Values { get; }

        void TransitionTo(IDictionary<Tuple<object, string>, IDefaultValue> defaultValues);

        IStateDefinitionDataWrapper UntypedStateDataWrapper { get; }
    }
}