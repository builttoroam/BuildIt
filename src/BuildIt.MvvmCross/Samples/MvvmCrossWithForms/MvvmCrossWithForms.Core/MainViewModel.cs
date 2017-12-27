using MvvmCross.Core.ViewModels;
using System;

namespace MvvmCrossWithForms.Core
{
    public class MainViewModel : MvxViewModel
    {
        public MainViewModel()
        {

        }
        public string WelcomeText => "Welcome to my data bound Xamarin Forms + MvvmCross application!";
    }

    public class TempApplication : MvxApplication
    {
        public override void Initialize()
        {
            RegisterNavigationServiceAppStart<MainViewModel>();
        }
    }
}
