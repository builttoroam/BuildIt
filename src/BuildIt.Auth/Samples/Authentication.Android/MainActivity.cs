using Android.App;
using Android.Content.PM;
using Android.OS;
using BuildIt.Auth;
using System;

namespace Authentication.Droid
{
    [Activity(Label = "Authentication", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
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
    [IntentFilter(new[] { Android.Content.Intent.ActionView }, DataScheme = "ext.auth", Categories = new[] { Android.Content.Intent.CategoryBrowsable, Android.Content.Intent.CategoryDefault })]
    public class ProductActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (Intent.Data != null)
            {
                UriLauncher.HandleUri(new Uri(Intent.DataString));
            }

            // StartActivity(new Intent(this, typeof(MainActivity)));
            Finish();
        }
    }
}