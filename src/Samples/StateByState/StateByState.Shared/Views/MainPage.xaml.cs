using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Diagnostics;
using Windows.UI.Xaml;
using BuildIt.Lifecycle;
using System;
using Windows.UI.Xaml.Controls;
using BuildIt.States;
using StateTriggerBase = BuildIt.States.StateTriggerBase;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace StateByState
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage: ICodeBehindViewModel<MainViewModel>

    {
        public enum TestStates
        {
            Base,
            Custom
        }

        private StateManager sm=new StateManager();

        public MainPage()
        {
            Data = new Wrapper<MainViewModel>(this);
            InitializeComponent();

            sm.Group<TestStates>()
                .DefineState(TestStates.Custom)
                .AddTrigger(new WindowSizeTrigger(this) {MinWidth = 700});

            (sm.StateGroups[typeof(TestStates)] as IStateGroupManager<TestStates>).StateChanged += MainPage_StateChanged;
        }

        private void MainPage_StateChanged(object sender, StateEventArgs<TestStates> e)
        {
            Debug.WriteLine($"State: {e.State}");
        }

        public MainViewModel CurrentViewModel => DataContext as MainViewModel;

        public Wrapper<MainViewModel> Data { get; }

        private void RegularCodebehindHandler(object sender, RoutedEventArgs e)
        {
           // Debug.WriteLine(ViewModel != null);
        }
    }


    public class WindowSizeTrigger : StateTriggerBase
    {

        public int MinWidth { get; set; }   

       
        public WindowSizeTrigger(Page page)
        {
            page.SizeChanged += Page_SizeChanged;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateIsActive(e.NewSize.Width>=MinWidth);
        }
    }

}
