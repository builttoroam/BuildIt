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

    public class StateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var type = value.GetType();
            var gtype = type.GetInterfaces().FirstOrDefault(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == typeof(IViewModelWithState<>));
            if (gtype != null)
            {
                var appStateType = typeof(AppStateWrapper<>).MakeGenericType(gtype.GetGenericArguments());
                return Activator.CreateInstance(appStateType, value);
            }

            return value;
            //var appState = value as IViewModelWithState<string>;
            //return new AppStateWrapper<string>(appState);

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }

}
