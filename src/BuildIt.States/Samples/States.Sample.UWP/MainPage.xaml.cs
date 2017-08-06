using System.Threading;
using System.Threading.Tasks;
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

        public enum Test2State
        {
            Base,
            State1,
            State2
        }


        /// <summary>
        /// Invoked when navigated to the page
        /// </summary>
        /// <param name="e">The navigation args</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            (DataContext as MainViewModel)?.StateManager.GoToState(LoadingStates.UILoading);



            var sm = new StateManager();
            sm.Group<Test2State>()
                .DefineState(Test2State.State1)
                .WhenChangedFrom(async cancel =>
                {
                    await Task.Delay(30000, cancel);
                })
                .DefineState(Test2State.State2);

            var cancelT = new CancellationTokenSource();
            await sm.GoToState(Test2State.State1);
            //Assert.AreEqual(Test2State.State1, sm.CurrentState<Test2State>());
            var waiter = sm.GoToState(Test2State.State2, false, cancelT.Token);
            cancelT.Cancel();
            await waiter;
            var current = sm.CurrentState<Test2State>();
            //Assert.AreEqual(Test2State.State2, sm.CurrentState<Test2State>());
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
