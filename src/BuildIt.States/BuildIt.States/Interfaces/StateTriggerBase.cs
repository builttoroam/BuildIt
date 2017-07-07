using System;

namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Base class for state trigger
    /// </summary>
    public class StateTriggerBase : IStateTriggerActivation, IStateTrigger
    {
        /// <summary>
        /// Event indicating that the active state of the trigger has changed
        /// </summary>
        public event EventHandler IsActiveChanged;

        private bool isActive;

        /// <summary>
        /// Updates the active state of the trigger
        /// </summary>
        /// <param name="newIsActive"></param>
        public void UpdateIsActive(bool newIsActive)
        {
            IsActive = newIsActive;
        }

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
    }
}