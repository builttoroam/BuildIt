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
            NavigationHelper.Register<ThirdStates,ThrirdOnePage>(ThirdStates.One);
            NavigationHelper.Register<ThirdStates,ThirdTwoPage>(ThirdStates.Two);
            NavigationHelper.Register<ThirdStates,ThirdThreePage>(ThirdStates.Three);
            NavigationHelper.Register<ThirdStates,ThirdFourPage>(ThirdStates.Four);
            await CurrentViewModel.Start();
        }

        public ThirdViewModel CurrentViewModel=>DataContext as ThirdViewModel;
    }
}
