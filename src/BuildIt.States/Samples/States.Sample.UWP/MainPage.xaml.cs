using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BuildIt.States;
using BuildIt.States.Interfaces;
using States.Sample.Core;
using BuildIt.States.UWP;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace States.Sample.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public IStateManager StateManager { get; } = new StateManager();

        public MainPage()
        {
            this.InitializeComponent();
            var vm = new MainViewModel();
            this.DataContext = vm;

            StateManager
                
                .Group<LoadingStates>()
                .DefineAllStates(this, LoadingUIStates)

                .Group<SizeStates>()
                .DefineAllStates(this,LayoutStates);

            StateManager.Bind(vm.StateManager);
        }


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
    }

}
