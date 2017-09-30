using BuildIt.ServiceLocation;
using BuildIt.States.Interfaces;
using BuildIt.States.Interfaces.StateData;
using System;

namespace BuildIt.Lifecycle.States.ViewModel
{
    /// <summary>
    /// Base view model which implements interfaces for registering dependencies and UI access
    /// </summary>
    public class BaseViewModel : NotifyBase, IRegisterDependencies, IRegisterForUIAccess, IIsAbleToBeBlocked
    {
        private bool isBlocked;

        /// <summary>
        /// Event to indicate that the blocked status has changed
        /// </summary>
        public event EventHandler IsBlockedChanged;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets whether state transitions (back) should be blocked
        /// </summary>
        public virtual bool IsBlocked
        {
            get => isBlocked;
            set
            {
                if (value == isBlocked)
                {
                    return;
                }

                isBlocked = value;
                OnIsBlockedChanged();
            }
        }

        /// <summary>
        /// Gets the UI Context
        /// </summary>
        public IUIExecutionContext UIContext { get; private set; }

        /// <summary>
        /// Register dependencies for injection
        /// </summary>
        /// <param name="container">The injection container</param>
        public virtual void RegisterDependencies(IDependencyContainer container)
        {
        }

        /// <summary>
        /// Registers the entity for UI access
        /// </summary>
        /// <param name="context">The UI Context to use for execution</param>
        public virtual void RegisterForUIAccess(IUIExecutionContext context)
        {
            UIContext = context;
        }

        /// <summary>
        /// Raises the IsBlockedChanged event
        /// </summary>
        protected void OnIsBlockedChanged()
        {
            IsBlockedChanged.SafeRaise(this);
        }
    }
}