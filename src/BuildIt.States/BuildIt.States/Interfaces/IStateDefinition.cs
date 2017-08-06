using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Properties and Methods that are defined for a state
    /// </summary>
    public interface IStateDefinition
    {
        /// <summary>
        /// Gets or sets gets the name of the state
        /// </summary>
        string StateName { get; set; }

        /// <summary>
        /// Gets a list of triggers that can be used to determine if the
        /// state should become the current state
        /// </summary>
        IList<IStateTrigger> Triggers { get; }

        /// <summary>
        /// Gets or sets method to be invoked when about to change away from the state
        /// The state transition can be cancelled
        /// </summary>
        Func<StateCancelEventArgs, Task> AboutToChangeFrom { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when about to change to the state (cancellable)
        /// </summary>
        Func<StateCancelEventArgs, Task> AboutToChangeTo { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when about to change to the state (cancellable)
        /// </summary>
        Func<string, StateCancelEventArgs, Task> AboutToChangeToWithData { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when the state transition has started (leaving this state)
        /// </summary>
        Func<CancellationToken, Task> ChangingFrom { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when the state transition to this state is about to start (arriving at this state)
        /// </summary>
        Func<CancellationToken, Task> ChangingTo { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when the state transition to this state is about to start (arriving at this state)
        /// </summary>
        Func<string, CancellationToken, Task> ChangingToWithData { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when the state transition has completed (left this state)
        /// </summary>
        Func<CancellationToken, Task> ChangedFrom { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when the state transition has completed (arriving at this state)
        /// </summary>
        Func<CancellationToken, Task> ChangedTo { get; set; }

        /// <summary>
        /// Gets or sets method to be invoked when the state transition has completed, with data (arriving at this state)
        /// </summary>
        Func<string, CancellationToken, Task> ChangedToWithJsonData { get; set; }

        /// <summary>
        /// Gets state values (ie properties values to be set)
        /// </summary>
        IList<IStateValue> Values { get; }

        /// <summary>
        /// Gets or sets gets any state data (eg view model) that might be associated with the state
        /// </summary>
        IStateDefinitionDataWrapper UntypedStateDataWrapper { get; set; }

        /// <summary>
        /// Implements the transition, setting values and recording the default values
        /// </summary>
        /// <param name="targets">The set of target elements to use in state transition</param>
        /// <param name="defaultValues">The set of default values to apply if a state doesn't define property value</param>
        void TransitionTo(IDictionary<string, object> targets, IDictionary<Tuple<object, string>, IDefaultValue> defaultValues);
    }
}