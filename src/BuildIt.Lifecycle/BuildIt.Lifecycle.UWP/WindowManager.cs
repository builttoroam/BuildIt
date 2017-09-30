using BuildIt.Lifecycle.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private void RegionManager_RegionIsClosed(object sender, DualParameterEventArgs<IRegionManager, IApplicationRegion> args)
        {
            var region = args.Parameter2;

            var view = RegionWindows.SafeValue(region.RegionId);
            view.Close();
        }

#pragma warning disable 1998 // Async required for Windows UWP support for multiple views

        private async void RegionManager_RegionCreated(object sender, DualParameterEventArgs<IRegionManager, IApplicationRegion> args)
#pragma warning restore 1998
        {
            var regionManager = args.Parameter1;
            var region = args.Parameter2;

            var isPrimary = RegionManager.IsPrimaryRegion(region);
            var newView = isPrimary
                ? CoreApplication.MainView
                : CoreApplication.CreateNewView();
            var newViewId = 0;
            var context = new UniversalUIContext(newView.Dispatcher);
            region.RegisterForUIAccess(context);

            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var controller = new WindowRegionController(Window.Current, regionManager, region, isPrimary);
                newViewId = controller.Start();
                RegionWindows[region.RegionId] = controller;
            });

            if (isPrimary)
            {
                return;
            }

            var viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
            Debug.WriteLine(viewShown);
        }
    }
}