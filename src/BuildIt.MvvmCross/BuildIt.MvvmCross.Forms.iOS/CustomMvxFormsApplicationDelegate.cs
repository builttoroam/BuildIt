using MvvmCross.Core.ViewModels;
using MvvmCross.Forms.iOS;
using MvvmCross.Forms.Views;
using MvvmCross.Platform;

namespace BuildIt.MvvmCross.Forms.iOS
{
    public class CustomMvxFormsApplicationDelegate: MvxFormsApplicationDelegate
    {
        protected virtual void MvxLoadApplication()
        {
            var startup = Mvx.Resolve<IMvxAppStart>();
            startup.Start();

            var presenter = Mvx.Resolve<IMvxFormsViewPresenter>();
            LoadApplication(presenter.FormsApplication);
        }
    }
}
