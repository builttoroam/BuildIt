using System;
using System.Threading.Tasks;
using Autofac;
using BuildIt.Lifecycle.States;
using BuildIt.Lifecycle.States.ViewModel;

namespace BuildIt.Lifecycle
{
    public class ApplicationRegion:IApplicationRegion
    {
        public event EventHandler CloseRegion;

        public string RegionId { get; set; }

#pragma warning disable 1998 // Task to allow for async override
        public virtual async Task RequestClose()
#pragma warning restore 1998
        {
        }

        protected IRegionManager Manager { get; private set; }

#pragma warning disable 1998 // Task to allow for async override
        public async Task Startup(IRegionManager manager)
#pragma warning restore 1998
        {
            Manager = manager;
            await CompleteStartup();
        }

#pragma warning disable 1998 // Task to allow for async override
        protected virtual async Task CompleteStartup()
#pragma warning restore 1998
        {
            
        }

        protected void OnCloseRegion()
        {
            CloseRegion.SafeRaise(this);
        }

        public virtual void RegisterDependencies(IContainer container)
        {
            
        }

        public UIExecutionContext UIContext { get; } = new UIExecutionContext();
        public void RegisterForUIAccess(IRequiresUIAccess manager)
        {
            if (manager == null) return;
            manager.UIContext.RunContext = UIContext?.RunContext;
        }
    }
}