using System.Collections.Generic;
using System.Reflection;
using Android.Content;
using Android.Widget;
using BuildIt.Config.Core.Services.Interfaces;
using BuildIt.Config.Impl.Common;
using Client.Android.Impl;
using Client.Core;
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

        protected override void InitializeIoC()
        {
            base.InitializeIoC();
            //Mvx.LazyConstructAndRegisterSingleton<IUserDialogService, UserDialogService>();
            Mvx.LazyConstructAndRegisterSingleton<IVersionService, VersionService>();                        
        }
    }
}