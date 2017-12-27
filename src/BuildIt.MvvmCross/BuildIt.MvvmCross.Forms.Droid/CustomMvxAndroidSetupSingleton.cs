using Android.Content;
using Android.OS;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Platform;
using MvvmCross.Forms.Droid.Platform;
using MvvmCross.Forms.Droid.Views;
using MvvmCross.Platform;
using MvvmCross.Platform.Droid;
using System;
using System.Reflection;

namespace BuildIt.MvvmCross.Forms.Droid
{
    public class CustomMvxAndroidSetupSingleton<TSetup> : MvxAndroidSetupSingleton
        where TSetup : MvxFormsAndroidSetup
    {
        protected override Type FindSetupType()
        {
            return typeof(TSetup);
        }

        public static MvxAndroidSetupSingleton EnsureSingletonAvailable(Context applicationContext)
        {
            if (Instance != null)
                return Instance;


            var instance = new CustomMvxAndroidSetupSingleton<TSetup>();
            instance.CreateSetup(applicationContext);
            return Instance;
        }
    }

    public class CustomMvxFormsAppCompatActivity<TSetup, TViewModel> : MvxFormsAppCompatActivity<TViewModel>
        where TSetup : MvxFormsAndroidSetup
        where TViewModel : class, IMvxViewModel
    {
        private Assembly _resourceAssembly;
        public CustomMvxFormsAppCompatActivity()
        {
            _resourceAssembly = this.GetType().Assembly;
        }

        protected override Assembly GetResourceAssembly()
        {
            return _resourceAssembly;
        }

        protected override void OnCreate(Bundle bundle)
        {
            // Required for proper Push notifications handling      
            var setupSingleton = CustomMvxAndroidSetupSingleton<TSetup>.EnsureSingletonAvailable(ApplicationContext);

            setupSingleton.EnsureInitialized();

            var setup = Mvx.Resolve<IMvxAndroidGlobals>() as IDroidExecutable;
            setup.UpdateExecutable(this.GetType());

            base.OnCreate(bundle);

            MvxLoadApplication();
        }

        protected virtual void MvxLoadApplication()
        {
            var startup = Mvx.Resolve<IMvxAppStart>();
            startup.Start();

            InitializeForms(bundle);
        }
    }
}
