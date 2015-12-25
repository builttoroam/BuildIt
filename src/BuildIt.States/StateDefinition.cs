using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BuildIt.States
{
    public class StateDefinition<TState> : IStateDefinition<TState>
        where TState : struct
    {
        public TState State { get; set; }

        public Func<CancelEventArgs, Task> AboutToChangeFrom { get; set; }
        public Func<Task> ChangingFrom { get; set; }
        public Func<Task> ChangedTo { get; set; }

        public IList<IStateValue> Values { get; } = new List<IStateValue>();

        public void TransitionTo(IDictionary<Tuple<object, string>, IDefaultValue> defaultValues)
        {
            var defaults = new Dictionary<Tuple<object, string>, IDefaultValue>(defaultValues);

            foreach (var visualStateValue in Values)
            {
                visualStateValue.TransitionTo(defaultValues);
                defaults.Remove(visualStateValue.Key);
            }

            foreach (var defValue in defaults)
            {
                defValue.Value.RevertToDefault();
            }
        }

    }
}