using System;
using System.Threading.Tasks;
using BuildIt.Lifecycle.Interfaces;
using BuildIt.ServiceLocation;
using BuildIt.States.Interfaces;

namespace BuildIt.Lifecycle.Regions
{
    /// <summary>
    /// Implements application region
    /// </summary>
    public class ApplicationRegion : IApplicationRegion
    {
        /// <summary>
        /// Event indicating that a region should be closed
        /// </summary>
        public event EventHandler CloseRegion;

        /// <summary>
        /// Event indicating tha the region is about to close (cancellable)
        /// </summary>
        public event EventHandler<CancelEventArgs> ClosingRegion;


        /// <summary>
        /// Gets the id of the region
        /// </summary>
        public string RegionId { get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the region manager associated with the region
        /// </summary>
        public IRegionManager Manager { get; private set; }

        /// <summary>
        /// Requests the region closes
        /// </summary>
        public async Task<bool> RequestClose()
        {
            var canClose = await OnClosingRegion();
            if (!canClose) return false;
            await OnCloseRegion();
            return true;
        }

        /// <summary>
        /// Starts the region
        /// </summary>
        /// <param name="manager">The manager to be associated with the region</param>
        /// <returns>Task to await</returns>
#pragma warning disable 1998 // Task to allow for async override
        public async Task Startup(IRegionManager manager)
#pragma warning restore 1998
        {
            Manager = manager;
            await CompleteStartup();
        }

        /// <summary>
        /// Completes startup
        /// </summary>
#pragma warning disable 1998 // Task to allow for async override
        protected virtual async Task CompleteStartup()
#pragma warning restore 1998
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected virtual Task<bool> OnClosingRegion()
        {
            var cancel = new CancelEventArgs();
            ClosingRegion.SafeRaise(this, cancel);
            return Task.FromResult(!cancel.Cancel);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual async Task OnCloseRegion()
        {
            CloseRegion.SafeRaise(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void RegisterDependencies(IDependencyContainer container)
        {
            if (!(this is IHasStates hasStates)) return;
            foreach (var stateGroup in hasStates.StateManager.StateGroups)
            {
                stateGroup.Value?.RegisterDependencies(container);
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

            if (!(this is IHasStates hasStates)) return;
            foreach (var stateGroup in hasStates.StateManager.StateGroups)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global - // Casting is possible 
                if (stateGroup.Value is IRegisterForUIAccess uiAccess)
                {
                    uiAccess.RegisterForUIAccess(context);
                }
            }
        }
    }
}