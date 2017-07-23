using BuildIt.States;
using BuildIt.States.Interfaces;
using BuildIt.States.UWP;
using States.Sample.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace States.Sample.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            var vm = new MainViewModel();
            DataContext = vm;

            StateManager

                .Group<LoadingStates>()
                .DefineAllStates(this, LoadingUIStates)

                .Group<SizeStates>()
                .DefineAllStates(this, LayoutStates);

            StateManager.Bind(vm.StateManager);
        }

        /// <summary>
        /// Gets state Manager instance
        /// </summary>
        public IStateManager StateManager { get; } = new StateManager();

        /// <summary>
        /// Invoked when navigated to the page
        /// </summary>
        /// <param name="e">The navigation args</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            (DataContext as MainViewModel)?.StateManager.GoToState(LoadingStates.UILoading);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel)?.StateManager.GoToState(LoadingStates.UILoading);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel)?.StateManager.GoToState(LoadingStates.UILoadingFailed);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel)?.StateManager.GoToState(LoadingStates.UILoaded);
        }

        private async void ItemClicked(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as RandomItem;
            var state = item.StateManager.CurrentState<ItemStates>();
            switch (state)
            {
                case ItemStates.Base:
                    state = ItemStates.IsEnabled;
                    break;
                case ItemStates.IsEnabled:
                    state = ItemStates.IsEnabledTwo;
                    break;
                case ItemStates.IsEnabledTwo:
                    state = ItemStates.IsNotEnabled;
                    break;
                case ItemStates.IsNotEnabled:
                    state = ItemStates.IsEnabled;
                    break;
            }

            await item.StateManager.GoToState(state);
        }
    }
}
