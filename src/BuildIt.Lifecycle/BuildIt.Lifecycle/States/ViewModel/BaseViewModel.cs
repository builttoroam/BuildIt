using System;
using BuildIt.ServiceLocation;
using BuildIt.States;
using BuildIt.States.Interfaces;
using BuildIt.States.Interfaces.StateData;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public class BaseViewModel:NotifyBase, IRegisterDependencies, IRegisterForUIAccess, IIsAbleToBeBlocked
    {
        private bool isBlocked = false;
        public IUIExecutionContext UIContext { get; private set; }
        public virtual void RegisterForUIAccess(IUIExecutionContext context)
        {
            UIContext = context;
        }

        public virtual void RegisterDependencies(IDependencyContainer container)
        {
        }

        public event EventHandler IsBlockedChanged;

        public bool IsBlocked
        {
            get { return isBlocked; }
            set
            {
                if (value == isBlocked) return;
                isBlocked = value;
                OnIsBlockedChanged();
            }
        }

        protected void OnIsBlockedChanged()
        {
            IsBlockedChanged.SafeRaise(this);
        }
    }
}
