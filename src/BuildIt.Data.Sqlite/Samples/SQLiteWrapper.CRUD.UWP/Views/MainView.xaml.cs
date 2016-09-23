using Windows.UI.Xaml;
using SQLiteWrapper.CRUD.Core.ViewModels;

namespace SQLiteWrapper.CRUD.UWP.Views
{
    public sealed partial class MainView
    {
        private MainViewModel CurrentViewModel => ViewModel as MainViewModel;

        public MainView()
        {
            this.InitializeComponent();
        }


        private async void CreateClicked(object sender, RoutedEventArgs e)
        {
            await CurrentViewModel.Create();
        }
        private async void ReadClicked(object sender, RoutedEventArgs e)
        {
            await CurrentViewModel.Read();
        }
        private async void UpdateClicked(object sender, RoutedEventArgs e)
        {
            await CurrentViewModel.Update();
        }
        private async void DeleteClicked(object sender, RoutedEventArgs e)
        {
            await CurrentViewModel.Delete();
        }
    }
}
