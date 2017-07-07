using System;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Trigger for when a state should be active
    /// </summary>
    public interface IStateTrigger
    {
        /// <summary>
        /// Event for when the trigger changes active state
        /// </summary>
        event EventHandler IsActiveChanged;

        /// <summary>
        /// The active state of the trigger (ie when the conditions are true)
        /// </summary>
        bool IsActive { get; }
    }
}