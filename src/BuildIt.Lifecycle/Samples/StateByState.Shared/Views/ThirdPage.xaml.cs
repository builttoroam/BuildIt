using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using BuildIt.Lifecycle;
using BuildIt.Lifecycle.States;
using BuildIt.States;


namespace StateByState
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ThirdPage 
    {
        public ThirdPage()
        {
            InitializeComponent();

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var frame = Split.FindName("InnerFrame") as Frame;

            var fn = new FrameNavigation<ThirdStates>(frame, CurrentViewModel.StateManager.StateGroups[typeof(ThirdStates)] as INotifyStateChanged<ThirdStates>);
            await CurrentViewModel.Start();
        }

        public ThirdViewModel CurrentViewModel=>DataContext as ThirdViewModel;

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentViewModel.One();
        }

        private void Button1_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentViewModel.Two();
        }

        private void Button2_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentViewModel.Three();
        }

        private void Button3_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentViewModel.Four();
        }
    }
}
