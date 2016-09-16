using Android.App;
using Android.OS;
using BuildIt.MvvmCross.ViewModels;

namespace Buildit.MvvmCross.Droid
{
    [Activity(Label = "Buildit.MvvmCross.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Something something Buildit.MvvmCross...
            var vm = new BaseViewModel();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
        }
    }
}

