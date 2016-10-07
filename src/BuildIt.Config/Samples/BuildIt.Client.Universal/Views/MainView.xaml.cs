using Windows.UI.Xaml.Controls;
using BuildIt.Client.Core.ViewModels;
using MvvmCross.WindowsUWP.Views;

namespace BuildIt.Client.Universal.Views
{
    public sealed partial class MainView : MvxWindowsPage
    {
        public MainViewModel CurrentViewModel => ViewModel as MainViewModel;

        public MainView()
        {
            this.InitializeComponent();
        }
    }
}
