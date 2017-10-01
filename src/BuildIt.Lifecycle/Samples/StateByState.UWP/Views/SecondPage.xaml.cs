using BuildIt.States;
using BuildIt.States.Interfaces;
using BuildIt.States.UWP;
using StateByState.Regions.Main;
using Windows.UI.Xaml;

namespace StateByState
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SecondPage : IHasStates
    {
        public SecondPage()
        {
            InitializeComponent();

            StateManager

                .Group<SecondStates>()
                .DefineAllStates(this, VisualStateGroup)

                .Group<SecondStates2>()
                .DefineAllStates(this, VisualStateGroup2);
        }

        /// <summary>
        /// Gets state Manager instance
        /// </summary>
        public IStateManager StateManager { get; } = new StateManager();

        public SecondViewModel CurrentViewModel => DataContext as SecondViewModel;

        private void ToFirst(object sender, RoutedEventArgs e)
        {
            CurrentViewModel.ToFirst();
        }

        private void ToSecond(object sender, RoutedEventArgs e)
        {
            CurrentViewModel.ToSecond();
        }

        private void ToThird(object sender, RoutedEventArgs e)
        {
            CurrentViewModel.ToThird();
        }

        private void Done(object sender, RoutedEventArgs e)
        {
            CurrentViewModel.Done();
        }

        private void XtoZ(object sender, RoutedEventArgs e)
        {
            CurrentViewModel.XtoZ();
        }

        private void YtoZ(object sender, RoutedEventArgs e)
        {
            CurrentViewModel.YtoZ();
        }

        private void ZtoY(object sender, RoutedEventArgs e)
        {
            CurrentViewModel.ZtoY();
        }

        private void YtoX(object sender, RoutedEventArgs e)
        {
            CurrentViewModel.YtoX();
        }
    }
}