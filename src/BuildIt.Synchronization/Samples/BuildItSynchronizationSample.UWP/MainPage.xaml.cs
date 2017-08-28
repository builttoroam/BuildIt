using BuildItSynchronizationSample.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BuildItSynchronizationSample.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private MainViewModel ViewModel => DataContext as MainViewModel;

        public MainPage()
        {
            this.InitializeComponent();
            var mainViewModel = new MainViewModel {RunOnUIThreadAction = a => RunOnUiThread(a)};
            DataContext = mainViewModel;
        }

        private async Task RunOnUiThread(Action action)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
        }
    }
}
