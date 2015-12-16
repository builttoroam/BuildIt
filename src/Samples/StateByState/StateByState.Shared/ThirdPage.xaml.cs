using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using BuildIt.Lifecycle;


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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var frame = Split.FindName("InnerFrame") as Frame;

            var fn = new FrameNavigation<ThirdStates, ThirdTransitions>(frame, CurrentViewModel);
            NavigationHelper.Register<ThirdStates,ThrirdOnePage>(ThirdStates.One);
            NavigationHelper.Register<ThirdStates,ThirdTwoPage>(ThirdStates.Two);
            NavigationHelper.Register<ThirdStates,ThirdThreePage>(ThirdStates.Three);
            NavigationHelper.Register<ThirdStates,ThirdFourPage>(ThirdStates.Four);
            await CurrentViewModel.Start();
        }

        public ThirdViewModel CurrentViewModel=>DataContext as ThirdViewModel;
    }
}
