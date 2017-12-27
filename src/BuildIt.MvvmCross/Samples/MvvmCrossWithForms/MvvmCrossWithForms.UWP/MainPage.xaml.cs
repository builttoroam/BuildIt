using MvvmCross.Core.ViewModels;
using MvvmCross.Forms.Views;
using MvvmCross.Platform;

namespace MvvmCrossWithForms.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            MvxLoadApplication();
        }
    }
}
