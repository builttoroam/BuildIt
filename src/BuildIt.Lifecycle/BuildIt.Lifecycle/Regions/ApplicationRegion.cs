using BuildIt.Lifecycle.Interfaces;
using BuildIt.ServiceLocation;
using BuildIt.States.Interfaces;
using System;
using System.Threading.Tasks;

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
        /// Gets or sets gets the id of the region
        /// </summary>
        public string RegionId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets the region manager associated with the region
        /// </summary>
        public IRegionManager Manager { get; private set; }

        /// <summary>
        /// Gets the UI Context for this region
        /// </summary>
        public IUIExecutionContext UIContext { get; private set; }

        /// <summary>
        /// Requests that a region be closed
        /// </summary>
        /// <returns>Task to await</returns>
        public async Task<bool> RequestClose()
        {
            var canClose = await OnClosingRegion();
            if (!canClose)
            {
                return false;
            }

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
        /// Registers all dependencies for this region
        /// </summary>
        /// <param name="container">The dependency container</param>
        public virtual void RegisterDependencies(IDependencyContainer container)
        {
#pragma warning disable SA1119 // Statement must not use unnecessary parenthesis - incorrect error
            if (!(this is IHasStates hasStates))
#pragma warning restore SA1119 // Statement must not use unnecessary parenthesis
            {
                return;
            }

            foreach (var stateGroup in hasStates.StateManager.StateGroups)
            {
                stateGroup.Value?.RegisterDependencies(container);
            }
        }

        /// <summary>
        /// Register for UI access
        /// </summary>
        /// <param name="context">The UI context</param>
        public virtual void RegisterForUIAccess(IUIExecutionContext context)
        {
            UIContext = context;

#pragma warning disable SA1119 // Statement must not use unnecessary parenthesis - Incorrect error
            if (!(this is IHasStates hasStates))
#pragma warning restore SA1119 // Statement must not use unnecessary parenthesis
            {
                return;
            }

            foreach (var stateGroup in hasStates.StateManager.StateGroups)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global - // Casting is possible
                if (stateGroup.Value is IRegisterForUIAccess uiAccess)
                {
                    uiAccess.RegisterForUIAccess(context);
                }
            }
        }

        /// <summary>
        /// Completes startup
        /// </summary>
        /// <returns>Task to await</returns>
#pragma warning disable 1998 // Task to allow for async override

        protected virtual async Task CompleteStartup()
#pragma warning restore 1998
        {
        }

        /// <summary>
        /// Raises closing event
        /// </summary>
        /// <returns>Task to await - result indicates if region should close</returns>
        protected virtual Task<bool> OnClosingRegion()
        {
            var cancel = new CancelEventArgs();
            ClosingRegion.SafeRaise(this, cancel);
            return Task.FromResult(!cancel.Cancel);
        }

        /// <summary>
        /// Raises close event
        /// </summary>
        /// <returns>Task to await</returns>
#pragma warning disable CS1998 // Task to allow for async override

        protected virtual async Task OnCloseRegion()
#pragma warning restore CS1998
        {
            CloseRegion.SafeRaise(this);
        }
    }
}