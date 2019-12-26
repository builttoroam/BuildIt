using BuildIt.States.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.States
{
    /// <summary>
    /// Defines information about a state.
    /// </summary>
    public abstract class BaseStateDefinition : IStateDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseStateDefinition"/> class.
        /// Internal constructor to prevent creating the state without a name.
        /// </summary>
        protected BaseStateDefinition()
        {
        }

        /// <summary>
        /// Gets or sets gets the name of the state (should be unique).
        /// </summary>
        public virtual string StateName { get; set; }

        /// <summary>
        /// Gets the triggers that are available for the state.
        /// </summary>
        public IList<IStateTrigger> Triggers { get; } = new List<IStateTrigger>();

        /// <summary>
        /// Gets or sets method to be invoked when about to change from the state (cancellable).
        /// </summary>
        public Func<StateCancelEventArgs, Task> AboutToChangeFrom { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when about to change to the state (cancellable).
        /// </summary>
        public Func<StateCancelEventArgs, Task> AboutToChangeTo { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when about to change to the state (cancellable).
        /// </summary>
        public Func<string, StateCancelEventArgs, Task> AboutToChangeToWithData { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when changing from the state (not cancellable).
        /// </summary>
        public Func<CancellationToken, Task> ChangingFrom { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when the state transition to this state is about to start (arriving at this state).
        /// </summary>
        public Func<CancellationToken, Task> ChangingTo { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when the state transition to this state is about to start (arriving at this state).
        /// </summary>
        public Func<string, CancellationToken, Task> ChangingToWithData { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when the state transition has completed (left this state).
        /// </summary>
        public Func<CancellationToken, Task> ChangedFrom { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when changed to the state.
        /// </summary>
        public Func<CancellationToken, Task> ChangedTo { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when changed to the state, with data.
        /// </summary>
        public Func<string, CancellationToken, Task> ChangedToWithJsonData { get; set; }

        /// <summary>
        /// Gets the set of properties to change when entering this state.
        /// </summary>
        public IList<IStateValue> Values { get; } = new List<IStateValue>();

        /// <summary>
        /// Gets or sets the state data wrapper - captures the type of data to be created along with the state.
        /// </summary>
        public IStateDefinitionDataWrapper UntypedStateDataWrapper { get; set; }

        /// <summary>
        /// Transitions to this state - invokes the property setters.
        /// </summary>
        /// <param name="targets">The set of target elements to use in state transition.</param>
        /// <param name="defaultValues">The default values to be applied if state doesn't define property values.</param>
        public void TransitionTo(IDictionary<string, object> targets, IDictionary<Tuple<object, string>, IDefaultValue> defaultValues)
        {
            var defaults = new Dictionary<Tuple<object, string>, IDefaultValue>(defaultValues);

            foreach (var visualStateValue in Values)
            {
                visualStateValue.TransitionTo(targets, defaultValues);
                defaults.Remove(visualStateValue.Key);
            }

            foreach (var defValue in defaults)
            {
                defValue.Value.RevertToDefault(targets);
            }
        }
    }
}