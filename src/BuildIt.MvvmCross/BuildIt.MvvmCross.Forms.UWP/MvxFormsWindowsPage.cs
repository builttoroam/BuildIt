using MvvmCross.Core.ViewModels;
using MvvmCross.Forms.Views;
using MvvmCross.Platform;
using Xamarin.Forms.Platform.UWP;

namespace BuildIt.MvvmCross.Forms.UWP
{
    public class MvxFormsWindowsPage : WindowsPage
    {
        protected virtual void MvxLoadApplication()
        {
            var start = Mvx.Resolve<IMvxAppStart>();
            start.Start();

            var presenter = Mvx.Resolve<IMvxFormsViewPresenter>();
            LoadApplication(presenter.FormsApplication);
        }
    }
}
