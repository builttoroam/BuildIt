using Windows.UI.Xaml.Controls;
using Client.Core.ViewModels;
using MvvmCross.WindowsUWP.Views;

namespace Client.Universal.Views
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
