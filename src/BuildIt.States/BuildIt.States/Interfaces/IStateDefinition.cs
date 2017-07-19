using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Properties and Methods that are defined for a state
    /// </summary>
    public interface IStateDefinition
    {
        /// <summary>
        /// Gets the name of the state
        /// </summary>
        string StateName { get; }

        /// <summary>
        /// Gets a list of triggers that can be used to determine if the
        /// state should become the current state
        /// </summary>
        IList<IStateTrigger> Triggers { get; }

        /// <summary>
        /// Gets or sets method to be invoked when about to change away from the state
        /// The state transition can be cancelled
        /// </summary>
        Func<CancelEventArgs, Task> AboutToChangeFrom { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when about to change to the state (cancellable)
        /// </summary>
        Func<CancelEventArgs, Task> AboutToChangeTo { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when about to change to the state (cancellable)
        /// </summary>
        Func<string, CancelEventArgs, Task> AboutToChangeToWithData { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when the state transition has started (leaving this state)
        /// </summary>
        Func<Task> ChangingFrom { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when the state transition to this state is about to start (arriving at this state)
        /// </summary>
        Func<Task> ChangingTo { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when the state transition to this state is about to start (arriving at this state)
        /// </summary>
        Func<string, Task> ChangingToWithData { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when the state transition has completed (left this state)
        /// </summary>
        Func<Task> ChangedFrom { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when the state transition has completed (arriving at this state)
        /// </summary>
        Func<Task> ChangedTo { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when the state transition has completed, with data (arriving at this state)
        /// </summary>
        Func<string, Task> ChangedToWithJsonData { get; set; }

        /// <summary>
        /// Gets state values (ie properties values to be set)
        /// </summary>
        IList<IStateValue> Values { get; }

        /// <summary>
        /// Gets any state data (eg view model) that might be associated with the state
        /// </summary>
        IStateDefinitionDataWrapper UntypedStateDataWrapper { get; }

        /// <summary>
        /// Implements the transition, setting values and recording the default values
        /// </summary>
        /// <param name="targets">The set of target elements to use in state transition</param>
        /// <param name="defaultValues">The set of default values to apply if a state doesn't define property value</param>
        void TransitionTo(IDictionary<string, object> targets, IDictionary<Tuple<object, string>, IDefaultValue> defaultValues);
    }
}