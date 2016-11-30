using Android.Content;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Droid.Platform;
using MvvmCross.Droid.Views;
using MvvmCross.Forms.Presenter.Droid;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;

namespace CognitiveServicesDemo.Droid
{
    class Setup :MvxAndroidSetup
    {
        public Setup(Context applicationContext)
: base(applicationContext)
        {
        }

        protected override IMvxApplication CreateApp()
        {
            return new App();
        }

        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }

        protected override IMvxAndroidViewPresenter CreateViewPresenter()
        {
            var presenter = new MvxFormsDroidPagePresenter();
            Mvx.RegisterSingleton<IMvxViewPresenter>(presenter);
            //Mvx.RegisterType<IPhotoPropertiesService, PhotoPropertiesService>();

            return presenter;
        }

        protected override void InitializeIoC()
        {
            base.InitializeIoC();

            //Mvx.LazyConstructAndRegisterSingleton<IPhotoPropertiesService, PhotoPropertiesService>();
        }

    }
}