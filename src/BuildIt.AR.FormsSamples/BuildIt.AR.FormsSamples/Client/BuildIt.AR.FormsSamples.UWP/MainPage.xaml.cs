using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Forms.Uwp.Presenters;
using MvvmCross.Platform;

namespace BuildIt.AR.FormsSamples.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            // Start MvvMCross
            var start = Mvx.Resolve<IMvxAppStart>();
            start.Start();
            var presenter = Mvx.Resolve<IMvxViewPresenter>() as MvxFormsUwpPagePresenter;
            if (presenter != null)
            {
                LoadApplication(presenter.FormsApplication);
            }
        }
    }
}
