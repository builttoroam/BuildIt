using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using BuildIt.Config.Core.Standard.Models;
using BuildIt.Config.Core.Standard.Services;
using BuildIt.Config.Core.Standard.Services.Interfaces;
using Client.Android.Impl;
using Client.Core.ViewModels;
using MvvmCross.Droid.Views;
using MvvmCross.Platform;

namespace Client.Android.Test
{
    [Activity(Label = "Client.Android.Test", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : MvxActivity<MainViewModel>
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var userDialogService = Mvx.Resolve<IUserDialogService>() as UserDialogService;
            if (userDialogService != null)
            {
                userDialogService.Context = this;
            }
            var versionService = Mvx.Resolve<IVersionService>() as VersionService;
            if (versionService != null)
            {
                versionService.Context = this;
            }

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += async (obj, sender) =>
            {
                if (ViewModel == null) return;

                await ViewModel?.GetAppConfig();
            };
        }
    }
}

