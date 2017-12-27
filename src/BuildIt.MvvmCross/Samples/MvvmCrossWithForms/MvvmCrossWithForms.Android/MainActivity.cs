
using Android.App;
using Android.Content.PM;
using Android.OS;
using BuildIt.MvvmCross.Forms.Droid;
using MvvmCrossWithForms.Core;

namespace MvvmCrossWithForms.Droid
{
    [Activity(Label = "MvvmCrossWithForms", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : 
        CustomMvxFormsAppCompatActivity<SetupFromViewModel<MainViewModel,App>, MainViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
        }
    }

}

