using System.Collections.Generic;
using System.Reflection;
using Android.Content;
using Android.Widget;
using BuildIt.Config.Core.Standard.Models;
using BuildIt.Config.Core.Standard.Services;
using BuildIt.Config.Core.Standard.Services.Interfaces;
using Client.Android.Impl;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Platform;
using MvvmCross.Droid.Shared.Presenter;
using MvvmCross.Droid.Views;
using MvvmCross.Platform;

namespace Client.Android.Test
{
    public class Setup : MvxAndroidSetup
    {
        public Setup(Context applicationContext) : base(applicationContext)
        {
        }

        protected override IMvxApplication CreateApp()
        {
            return new App();
        }

        //protected override IEnumerable<Assembly> AndroidViewAssemblies => new List<Assembly>(base.AndroidViewAssemblies)
        //{
        //    typeof(NavigationView).Assembly,
        //    typeof(FloatingActionButton).Assembly,
        //    typeof(Toolbar).Assembly,
        //    typeof(DrawerLayout).Assembly,
        //    typeof(ViewPager).Assembly
        //    //typeof(MvvmCross.Droid.Support.V7.RecyclerView.MvxRecyclerView).Assembly
        //};

        /// <summary>
        /// This is very important to override. The default view presenter does not know how to show fragments!
        /// </summary>
        //protected override IMvxAndroidViewPresenter CreateViewPresenter()
        //{
        //    var mvxFragmentsPresenter = new MvxFragmentsPresenter(AndroidViewAssemblies);
        //    Mvx.RegisterSingleton<IMvxAndroidViewPresenter>(mvxFragmentsPresenter);
        //    return mvxFragmentsPresenter;
        //}

        protected override void InitializeIoC()
        {
            base.InitializeIoC();
            Mvx.LazyConstructAndRegisterSingleton<IUserDialogService, UserDialogService>();
            Mvx.LazyConstructAndRegisterSingleton<IVersionService, VersionService>();
            Mvx.LazyConstructAndRegisterSingleton<IAppConfigurationEndpointService>(() => new AppConfigurationEndpointService(new AppConfigurationRoutingModel()
            {
                Prefix = "test1",
                Controller = "configuration"
            }));
            Mvx.LazyConstructAndRegisterSingleton<IAppConfigurationService, AppConfigurationService>();
        }
    }
}