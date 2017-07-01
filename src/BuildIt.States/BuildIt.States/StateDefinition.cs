using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    public class StateDefinition : IStateDefinition
    {
        public virtual string StateName { get; }

        public IList<IStateTrigger> Triggers { get; }  = new List<IStateTrigger>();

        public Func<CancelEventArgs, Task> AboutToChangeFrom { get; set; }
        public Func<Task> ChangingFrom { get; set; }
        public Func<Task> ChangedTo { get; set; }

        public Func<string, Task> ChangedToWithJsonData { get; set; }
        
        public IList<IStateValue> Values { get; } = new List<IStateValue>();

        protected StateDefinition()
        {
            
        }
        public StateDefinition(string stateName)
        {
            StateName = stateName;
        }


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

        public IStateDefinitionDataWrapper UntypedStateDataWrapper { get; set; }
    }
}