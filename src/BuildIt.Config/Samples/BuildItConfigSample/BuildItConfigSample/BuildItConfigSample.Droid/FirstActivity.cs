using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BuildItConfigSample.ViewModels;
using MvvmCross.Droid.Views;
using MvvmCross.Platform;
using MvvmCross.Platform.Droid.Platform;
using Plugin.VersionTracking;

namespace BuildItConfigSample.Droid
{
    [Activity(Label = "FirstActivity", MainLauncher = true)]
    public class FirstActivity : MvxActivity<FirstViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            UserDialogs.Init(() => Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity);
            base.OnCreate(bundle);
        }

        protected override void OnViewModelSet()
        {            
            base.OnViewModelSet();
            SetContentView(Resource.Layout.Main);
        }
    }
}