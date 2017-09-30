using Windows.UI.Xaml;
using StateByState.Regions.Secondary;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StateByState.Shared
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SeparatePage
    {
        public SeparatePage()
        {
            this.InitializeComponent();
        }

        public SecondaryMainViewModel CurrentViewModel => DataContext as SecondaryMainViewModel;

        private void EndClick(object sender, RoutedEventArgs e)
        {
            CurrentViewModel.IsDone();
        }
    }
}