using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using BuildIt.General.UI;
using BuildIt.States;
using BuildIt.States.Interfaces;
using StateByState.Regions.Main;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace StateByState.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : ICodeBehindViewModel<MainViewModel>, IHasStates

    {
        public MainPage()
        {
            Data = new ContextWrapper<MainViewModel>(this);
            InitializeComponent();

            StateManager
                .Group<TestStates>()
                .DefineState(TestStates.Custom)
                .AddTrigger(new WindowSizeTrigger(this) { MinWidth = 700 });

            var grp = (from sg in StateManager.StateGroups.Values
                       let tg = sg as INotifyTypedStateChange<TestStates>
                       where tg != null
                       select tg).FirstOrDefault();
            if (grp != null)
            {
                grp.TypedStateChanged += MainPage_StateChanged;
            }
        }

        public enum TestStates
        {
            Base,
            Custom
        }

        public MainViewModel CurrentViewModel => DataContext as MainViewModel;

        public ContextWrapper<MainViewModel> Data { get; }

        /// <summary>
        /// Gets state Manager instance
        /// </summary>
        public IStateManager StateManager { get; } = new StateManager();

        private void Fourth(object sender, RoutedEventArgs e)
        {
            Data.ViewModel.Fourth();
        }

        private void MainPage_StateChanged(object sender, ITypedStateEventArgs<TestStates> e)
        {
            Debug.WriteLine($"State: {e.StateName}");
        }

        private void RegularCodebehindHandler(object sender, RoutedEventArgs e)
        {
            // Debug.WriteLine(ViewModel != null);
        }

        private void Spawn(object sender, RoutedEventArgs e)
        {
            Data.ViewModel.Spawn();
        }

        private void Test(object sender, RoutedEventArgs e)
        {
            Data.ViewModel.Test();
        }

        private void Three(object sender, RoutedEventArgs e)
        {
            Data.ViewModel.Three();
        }
    }
}