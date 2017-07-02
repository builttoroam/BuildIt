using System;
using System.Threading.Tasks;
using BuildIt.ServiceLocation;
using BuildIt.States;
using BuildIt.States.Interfaces;

namespace BuildIt.Lifecycle
{
    /// <summary>
    /// 
    /// </summary>
    public class StateAwareApplicationRegion : ApplicationRegion, IHasStates
    {
        /// <summary>
        /// 
        /// </summary>
        public IStateManager StateManager { get; } = new StateManager();
    }

    /// <summary>
    /// 
    /// </summary>
    public class ApplicationRegion:IApplicationRegion
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler CloseRegion;

        /// <summary>
        /// 
        /// </summary>
        public string RegionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
#pragma warning disable 1998 // Task to allow for async override
        public virtual async Task RequestClose()
#pragma warning restore 1998
        {
            OnCloseRegion();
        }

        /// <summary>
        /// 
        /// </summary>
        public IRegionManager Manager { get; private set; }

        /// <summary>
        /// 
        /// </summary>
#pragma warning disable 1998 // Task to allow for async override
        public async Task Startup(IRegionManager manager)
#pragma warning restore 1998
        {
            Manager = manager;
            await CompleteStartup();
        }

        /// <summary>
        /// 
        /// </summary>
#pragma warning disable 1998 // Task to allow for async override
        protected virtual async Task CompleteStartup()
#pragma warning restore 1998
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        protected void OnCloseRegion()
        {
            CloseRegion.SafeRaise(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void RegisterDependencies(IDependencyContainer container)
        {
            var hasSM = this as IHasStates;
            if (hasSM == null) return;
            foreach (var stateGroup in hasSM.StateManager.StateGroups)
            {
                (stateGroup.Value as IRegisterDependencies)?.RegisterDependencies(container);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IUIExecutionContext UIContext { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual void RegisterForUIAccess(IUIExecutionContext context)
        {
            UIContext = context;

            var hasSM = this as IHasStates;
            if (hasSM == null) return;
            foreach (var stateGroup in hasSM.StateManager.StateGroups)
            {
                (stateGroup.Value as IRegisterForUIAccess)?.RegisterForUIAccess(context);
            }
        }
    }
}