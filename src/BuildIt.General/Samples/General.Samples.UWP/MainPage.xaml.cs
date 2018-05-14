using General.Samples.Core;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace General.Samples.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            DataContext = new MainViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            (DataContext as MainViewModel).LoadBob();
        }

        private void BobClick(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).LoadBob();
        }
        private void FredClick(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).LoadFred();

        }

        private void Bob2Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).LoadBob2();
        }

        private void MutateClick(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).Mutate();

        }
    }

}
