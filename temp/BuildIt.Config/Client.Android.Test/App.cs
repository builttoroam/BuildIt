using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace Client.Android.Test
{
    public class App : MvxApplication
    {
        public App()
        {

        }


        public override void Initialize()
        {
            base.Initialize();
            RegisterAppStart<MainViewModel>();
        }
    }
}