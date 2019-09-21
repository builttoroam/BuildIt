using System;

namespace BuildIt.States.Interfaces.StateData
{
    /// <summary>
    /// Entity determines whether it is blocked.
    /// </summary>
    public interface IIsAbleToBeBlocked
    {
        /// <summary>
        /// Blocked status has changed
        /// </summary>
        event EventHandler IsBlockedChanged;

        /// <summary>
        /// Gets a value indicating whether blocked state.
        /// </summary>
        bool IsBlocked { get; }
    }
}