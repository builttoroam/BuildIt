using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BuildIt.Lifecycle.States;
using BuildIt.States;

namespace BuildIt.Lifecycle
{
    public class WindowManager
    {
        private IRegionManager RegionManager { get; }

        private IDictionary<string, CoreWindow> Windows { get; }=new Dictionary<string, CoreWindow>(); 

        public WindowManager(IHasRegionManager root)
        {
            RegionManager = root.RegionManager;
            RegionManager.RegionCreated = RegionManager_RegionCreated;
            RegionManager.RegionIsClosing = RegionManager_RegionIsClosing;
        }

        private void RegionManager_RegionIsClosing(IRegionManager sender, IApplicationRegion e)
        {
            var view = Windows.SafeValue<string, CoreWindow, CoreWindow>(e.RegionId);
            view.Close();

        }

#pragma warning disable 1998 // Async required for Windows UWP support for multiple views
        private async void RegionManager_RegionCreated(IRegionManager sender, IApplicationRegion e)
#pragma warning restore 1998
        {
#if WINDOWS_UWP

            var isPrimary = RegionManager.IsPrimaryRegion(e);
            var newView = isPrimary
                ? CoreApplication.MainView
                : CoreApplication.CreateNewView();
            var newViewId = 0;
            var context = new UniversalUIContext(newView.Dispatcher);
            e.RegisterForUIAccess(context);



            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,  () =>
            {
                var frame = new Frame();

                var region = e;

                var hasStates = region as IHasStates;
                var sm = (region as IHasStates)?.StateManager;
                if (sm != null)
                {

                    var groups = sm.StateGroups;
                    var inotifier = typeof(INotifyStateChanged<>);
                    foreach (var stateGroup in groups)
                    {
                        var groupNotifier = inotifier.MakeGenericType(stateGroup.Key);
                        if (stateGroup.Value.GetType().GetTypeInfo().ImplementedInterfaces.Contains(groupNotifier))
                        {
                            var fnt = typeof(FrameNavigation<>).MakeGenericType(stateGroup.Key);
                            var fn = Activator.CreateInstance(fnt, frame, stateGroup.Value);
                        }
                    }
                }

                //var interfaces = region.GetType().GetInterfaces();
                //foreach (var it in interfaces)
                //{
                //    if (it.IsConstructedGenericType && 
                //    it.GetGenericTypeDefinition() == typeof(IHasViewModelStateManager<,>))
                //    {
                //        var args = it.GenericTypeArguments;
                //        var fnt = typeof (FrameNavigation<,>).MakeGenericType(args);
                //        var fn = Activator.CreateInstance(fnt, frame, region);//, string.Empty);
                //    }
                //}


                Window.Current.Content = frame;

                region.CloseRegion += Region_CloseRegion;

                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;

                Windows[region.RegionId] = Window.Current.CoreWindow;

                if (isPrimary)
                {
                    region.Startup(sender as IRegionManager);
                }
                else { 
                Task.Run(async () => await region.Startup(sender as IRegionManager));
                }


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