using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using BuildIt.Lifecycle;
using BuildIt.Lifecycle.States;
using BuildIt.States;
using BuildIt.States.Interfaces;
using System.Linq;

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

            var sm = CurrentViewModel.StateManager;
            var grp = (from sg in sm.StateGroups.Values.OfType<IStateGroup>()
                let tg = sg.GroupDefinition as INotifyTypedStateChange<ThirdStates>
                where tg != null
                select tg).FirstOrDefault();

            var fn = new FrameNavigation<ThirdStates>(frame, grp);// CurrentViewModel.StateManager.StateGroups[typeof(ThirdStates)] as INotifyStateChanged<ThirdStates>);
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
