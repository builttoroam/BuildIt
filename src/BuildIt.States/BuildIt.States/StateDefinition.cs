using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    /// <summary>
    /// Defines information about a state
    /// </summary>
    public class StateDefinition : IStateDefinition
    {
        /// <summary>
        /// Gets the name of the state (should be unique)
        /// </summary>
        public virtual string StateName { get; }

        /// <summary>
        /// Gets the triggers that are available for the state
        /// </summary>
        public IList<IStateTrigger> Triggers { get; } = new List<IStateTrigger>();

        /// <summary>
        /// Gets or sets method to be invoked when about to change from the state (cancellable)
        /// </summary>
        public Func<CancelEventArgs, Task> AboutToChangeFrom { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when about to change to the state (cancellable)
        /// </summary>
        public Func<CancelEventArgs, Task> AboutToChangeTo { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when about to change to the state (cancellable)
        /// </summary>
        public Func<string, CancelEventArgs, Task> AboutToChangeToWithData { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when changing from the state (not cancellable)
        /// </summary>
        public Func<Task> ChangingFrom { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when the state transition to this state is about to start (arriving at this state)
        /// </summary>
        public Func<Task> ChangingTo { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when the state transition to this state is about to start (arriving at this state)
        /// </summary>
        public Func<string, Task> ChangingToWithData { get; set; }


        /// <summary>
        /// Gets or sets method to be invoked when the state transition has completed (left this state)
        /// </summary>
        public Func<Task> ChangedFrom { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when changed to the state
        /// </summary>
        public Func<Task> ChangedTo { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when changed to the state, with data
        /// </summary>
        public Func<string, Task> ChangedToWithJsonData { get; set; }

        /// <summary>
        /// Gets the set of properties to change when entering this state
        /// </summary>
        public IList<IStateValue> Values { get; } = new List<IStateValue>();

        /// <summary>
        /// Initializes a new instance of the <see cref="StateDefinition"/> class.
        /// Internal constructor to prevent creating the state without a name
        /// </summary>
        protected StateDefinition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateDefinition"/> class.
        /// Creates an instance of the state with a name
        /// </summary>
        /// <param name="stateName">The name of the state</param>
        public StateDefinition(string stateName)
        {
            StateName = stateName;
        }

        /// <summary>
        /// Transitions to this state - invokes the property setters
        /// </summary>
        /// <param name="defaultValues"></param>
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

        /// <summary>
        /// Gets or sets the state data wrapper - captures the type of data to be created along with the state
        /// </summary>
        public IStateDefinitionDataWrapper UntypedStateDataWrapper { get; set; }
    }
}