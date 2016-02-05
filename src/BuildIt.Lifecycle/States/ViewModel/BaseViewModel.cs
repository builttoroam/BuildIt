using System;
using System.Diagnostics.Contracts;
using Autofac;
using BuildIt.States;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public class BaseViewModel:NotifyBase, ICanRegisterDependencies, IRegisterForUIAccess, IIsAbleToBeBlocked
    {
        private bool isBlocked = false;
        public IUIExecutionContext UIContext { get; private set; }
        public virtual void RegisterForUIAccess(IUIExecutionContext context)
        {
            UIContext = context;
        }

        public virtual void RegisterDependencies(IContainer container)
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
