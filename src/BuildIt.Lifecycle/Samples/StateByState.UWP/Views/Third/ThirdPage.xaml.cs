using BuildIt.States;
using BuildIt.States.Interfaces;
using BuildIt.States.UWP;
using StateByState.Regions.Main;
using StateByState.Regions.Main.Third;
using Windows.UI.Xaml;

namespace StateByState.Views.Third
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ThirdPage : IHasStates
    {
        public ThirdPage()
        {
            InitializeComponent();

            StateManager
                .Group<ThirdStates>()
                .DefineAllStates(this, ThirdVS);
        }

        public IStateManager StateManager { get; } = new StateManager();

        public ThirdViewModel CurrentViewModel => DataContext as ThirdViewModel;

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