using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BuildIt.Lifecycle.States;

namespace BuildIt.Lifecycle
{
    public class WindowManager
    {
        private IRegionManager RegionManager { get; }

        private IDictionary<string, CoreWindow> Windows { get; }=new Dictionary<string, CoreWindow>(); 

        public WindowManager(IHasRegionManager root)
        {
            RegionManager = root.RegionManager;
            RegionManager.RegionCreated += RegionManager_RegionCreated;
            RegionManager.RegionIsClosing += RegionManager_RegionIsClosing;
        }

        private void RegionManager_RegionIsClosing(object sender, ParameterEventArgs<IApplicationRegion> e)
        {
            var view = Windows.SafeDictionaryValue<string, CoreWindow, CoreWindow>(e.Parameter1.RegionId);
            view.Close();

        }

#pragma warning disable 1998 // Async required for Windows UWP support for multiple views
        private async void RegionManager_RegionCreated(object sender, ParameterEventArgs<IApplicationRegion> e)
#pragma warning restore 1998
        {
#if WINDOWS_UWP

            var isPrimary = RegionManager.IsPrimaryRegion(e.Parameter1);
            var newView = isPrimary
                ? CoreApplication.MainView
                : CoreApplication.CreateNewView();
            var newViewId = 0;
            e.Parameter1.UIContext.RunContext = new UniversalUIContext(newView.Dispatcher);
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var frame = new Frame();

                var region = e.Parameter1;

                var interfaces = region.GetType().GetInterfaces();
                foreach (var it in interfaces)
                {
                    if (it.IsConstructedGenericType && 
                    it.GetGenericTypeDefinition() == typeof(IHasViewModelStateManager<,>))
                    {
                        var args = it.GenericTypeArguments;
                        var fnt = typeof (FrameNavigation<,>).MakeGenericType(args);
                        var fn = Activator.CreateInstance(fnt, frame, region);//, string.Empty);
                    }
                }


                Window.Current.Content = frame;

                region.CloseRegion += Region_CloseRegion;

                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;

                Windows[region.RegionId] = Window.Current.CoreWindow;

                Task.Run(async () => await region.Startup(sender as IRegionManager));


            });

            if (!isPrimary)
            {
                var viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
                Debug.WriteLine(viewShown);
            }
#endif
        }

        private void Region_CloseRegion(object sender, EventArgs e)
        {
            Windows[(sender as IApplicationRegion).RegionId].Close();
        }
    }
}