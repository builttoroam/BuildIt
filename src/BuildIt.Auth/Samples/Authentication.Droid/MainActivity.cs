﻿using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using BuildIt.Auth;

namespace Authentication.Droid
{
    [Activity(Label = "Authentication", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }

    [Activity(Label = "Log in")]
    [IntentFilter(new[] { Intent.ActionView }, DataScheme = "ext.auth",  Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault })]
    public class ProductActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (Intent.Data != null)
            {
                UriLauncher.HandleUri(new Uri(Intent.DataString));
            }

            //StartActivity(new Intent(this, typeof(MainActivity)));
            Finish();
        }
    }
}
