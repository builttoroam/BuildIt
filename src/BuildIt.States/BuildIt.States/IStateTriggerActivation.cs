using System;
using System.Diagnostics.Contracts;

namespace BuildIt.States
{
    public interface IStateTriggerActivation
    {
        void UpdateIsActive(bool isActive);
    }

    public interface IStateTrigger
    {
        event EventHandler IsActiveChanged;
        bool IsActive { get; }
    }

    public class StateTriggerBase:IStateTriggerActivation, IStateTrigger
    {
        public event EventHandler IsActiveChanged;

        private bool isActive;

        public void UpdateIsActive(bool newIsActive)
        {
            IsActive = newIsActive;
        }


        public bool IsActive
        {
            get { return isActive; }
            private set
            {
                if (isActive == value) return;
                isActive = value;
                IsActiveChanged.SafeRaise(this);
            }
        }
    }

    
}