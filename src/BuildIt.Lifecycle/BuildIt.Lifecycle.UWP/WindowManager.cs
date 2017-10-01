using BuildIt.Lifecycle.Interfaces;
using BuildIt.Lifecycle.States;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace BuildIt.Lifecycle
{
    /// <summary>
    /// Manages windows for an application
    /// </summary>
    public class WindowManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowManager"/> class.
        /// </summary>
        /// <param name="root">The root of the application that has a region manager</param>
        public WindowManager(IHasRegionManager root)
        {
            RegionManager = root.RegionManager;
            RegionManager.RegionCreated += RegionManager_RegionCreated;
            RegionManager.RegionIsClosed += RegionManager_RegionIsClosed;
        }

        /// <summary>
        /// Gets the region manager that manages the regions for the application
        /// </summary>
        private IRegionManager RegionManager { get; }

        /// <summary>
        /// Gets the mapping between region id and window
        /// </summary>
        private IDictionary<string, WindowRegionController> RegionWindows { get; } = new Dictionary<string, WindowRegionController>();

        private async void RegionManager_RegionCreated(object sender, DualParameterEventArgs<IRegionManager, IApplicationRegion> args)
        {
            var regionManager = args.Parameter1;
            var region = args.Parameter2;
            $"New region created, now creating new Window for region {region.GetType().Name}".LogLifecycleInfo();

            // If this is the primary region, need to use the main view of the application
            // otherwise create a new window
            var isPrimary = RegionManager.IsPrimaryRegion(region);
            var newView = isPrimary
                ? CoreApplication.MainView
                : CoreApplication.CreateNewView();
            $"Is primary window {isPrimary}".LogLifecycleInfo();

            // Make sure that the region is set to run UI tasks on the correct thread
            // using a UIContext to host a reference to the view dispatcher
            var context = new UniversalUIContext(newView.Dispatcher);
            region.RegisterForUIAccess(context);
            "Registered for UI access".LogLifecycleInfo();

            var newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Create a new Window controller that will manage the window itselt
                var controller = new WindowRegionController(Window.Current, regionManager, region, isPrimary);
                newViewId = controller.Start();
                RegionWindows[region.RegionId] = controller;
                $"Window is wrapped in a window region controller - View Id {newViewId}".LogLifecycleInfo();
            });

            // If this is the primary window, we don't need to do anything else
            // otherwise, we need to attempt to show the window
            if (isPrimary)
            {
                return;
            }

            var viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
            $"View is shown - {viewShown}".LogLifecycleInfo();
        }

        private void RegionManager_RegionIsClosed(object sender, DualParameterEventArgs<IRegionManager, IApplicationRegion> args)
        {
            var region = args.Parameter2;
            $"Region has closed {region.RegionId}".LogLifecycleInfo();

            var view = RegionWindows.SafeValue(region.RegionId);
            view?.Close();
        }
    }
}