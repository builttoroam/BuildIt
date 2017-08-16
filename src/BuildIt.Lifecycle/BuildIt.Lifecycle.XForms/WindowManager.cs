using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using BuildIt.Lifecycle.Interfaces;
using BuildIt.Lifecycle.States;
using BuildIt.States;
using BuildIt.States.Interfaces;
using Xamarin.Forms;

namespace BuildIt.Lifecycle
{
    public class CustomNavigationPage : NavigationPage
    {
        public IApplicationRegion ActiveRegion { get; set; }
        protected override bool OnBackButtonPressed()
        {
            var reg = ActiveRegion as IHasStates;
            if (reg?.StateManager.PreviousStateExists ?? false)
            {
                reg.StateManager.GoBackToPreviousState();
                return true;
            }

            return base.OnBackButtonPressed();
        }
    }

    public class WindowManager
    {
        private IRegionManager RegionManager { get; }

        //private IDictionary<string, CoreWindow> Windows { get; }=new Dictionary<string, CoreWindow>(); 

            public CustomNavigationPage NavigationRoot { get; }


        public WindowManager(CustomNavigationPage navRoot, IHasRegionManager root)
        {
            NavigationRoot=navRoot;
            RegionManager = root.RegionManager;
            RegionManager.RegionCreated += RegionManager_RegionCreated;
            RegionManager.RegionIsClosed+= RegionManager_RegionIsClosing;
        }

        private void RegionManager_RegionIsClosing(object source, DualParameterEventArgs<IRegionManager, IApplicationRegion> args)
        {
            //var view = Windows.SafeDictionaryValue<string, CoreWindow, CoreWindow>(e.Parameter1.RegionId);
            //view.Close();

        }

#pragma warning disable 1998 // Async required for Windows UWP support for multiple views
        private async void RegionManager_RegionCreated(object source, DualParameterEventArgs<IRegionManager, IApplicationRegion> args)
#pragma warning restore 1998
        {
            IRegionManager sender = args.Parameter1;
            IApplicationRegion e = args.Parameter2;
            //var newViewId = 0;
            e.RegisterForUIAccess(new FormsUIContext());

            var region = e;

            if (RegionManager.IsPrimaryRegion(region))
            {
                InitialiseRegion(region);
            }
            else
            {
                await e.UIContext.RunAsync(() =>
                {
                    InitialiseRegion(region);
                });
            }

            //if (!isPrimary)
            //{
            //    var viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
            //    Debug.WriteLine(viewShown);
            //}
        }

        private async void InitialiseRegion(IApplicationRegion region)
        {

            var sm = (region as IHasStates)?.StateManager;
            if (sm != null)
            {

                var groups = sm.StateGroups;
                var inotifier = typeof(INotifyTypedStateChanged<>);
                foreach (var stateGroup in groups)
                {
                    // TODO: FIX
                    //var groupNotifier = inotifier.MakeGenericType(stateGroup.Key);
                    //if (stateGroup.Value.GetType().GetTypeInfo().ImplementedInterfaces.Contains(groupNotifier))
                    //{
                    //    var fnt = typeof(FrameNavigation<>).MakeGenericType(stateGroup.Key);
                    //    var fn = Activator.CreateInstance(fnt, NavigationRoot, sm);
                    //}
                }
            }

            //foreach (var stateGroup in sm.StateGroups)
            //{
            //    var fnt = typeof(FrameNavigation<,>).MakeGenericType(stateGroup.Key);
            //    var fn = Activator.CreateInstance(fnt, NavigationRoot, region);
            //}

            //var interfaces = region.GetType().GetTypeInfo().ImplementedInterfaces;//.GetInterfaces();
            //foreach (var it in interfaces)
            //{
            //    if (it.IsConstructedGenericType && 
            //    it.GetGenericTypeDefinition() == typeof(IHasViewModelStateManager<,>))
            //    {
            //        var args = it.GenericTypeArguments;
            //        var fnt = typeof (FrameNavigation<,>).MakeGenericType(args);
            //        var fn = Activator.CreateInstance(fnt, NavigationRoot, region);//, string.Empty);
            //    }
            //}


            //NavigationRoot.Navigation.PushAsync()
            //Application.MainPage = navRoot;
            //Window.Current.Content = frame;

            region.CloseRegion += Region_CloseRegion;

            //Window.Current.Activate();

            //newViewId = ApplicationView.GetForCurrentView().Id;

            //Windows[region.RegionId] = Window.Current.CoreWindow;

            NavigationRoot.ActiveRegion = region;

            if (RegionManager.IsPrimaryRegion(region))
            {
                await region.Startup(RegionManager);
            }
            else
            {
#pragma warning disable 4014 // Pushed intentionally to different thread without blocking this code
                Task.Run(async () => await region.Startup(RegionManager));
#pragma warning restore 4014
            }

        }

        private void Region_CloseRegion(object sender, EventArgs e)
        {
            //Windows[(sender as IApplicationRegion).RegionId].Close();
        }
    }


}