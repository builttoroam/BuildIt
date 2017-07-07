using System;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Base class for state trigger
    /// </summary>
    public class StateTriggerBase : IStateTriggerActivation, IStateTrigger
    {
        private bool isActive;

        /// <summary>
        /// Event indicating that the active state of the trigger has changed
        /// </summary>
        public event EventHandler IsActiveChanged;

        /// <summary>
        /// Gets a value indicating whether getss/Sets the active state of the trigger
        /// </summary>
        public bool IsActive
        {
            get => isActive;
            private set
            {
                if (isActive == value)
                {
                    return;
                }

                isActive = value;
                IsActiveChanged.SafeRaise(this);
            }
        }

        /// <summary>
        /// Updates the active state of the trigger
        /// </summary>
        /// <param name="newIsActive">The new active state to set</param>
        public void UpdateIsActive(bool newIsActive)
        {
            IsActive = newIsActive;
        }
    }
}