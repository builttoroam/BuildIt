using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Diagnostics;
using Windows.UI.Xaml;
using BuildIt.Lifecycle;
using System;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace StateByState
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage: ICodeBehindViewModel<MainViewModel>

    {
        public MainPage()
        {
            Data = new Wrapper<MainViewModel>(this);
            InitializeComponent();
        }
        public MainViewModel CurrentViewModel => DataContext as MainViewModel;

        public Wrapper<MainViewModel> Data { get; }

        private void RegularCodebehindHandler(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(ViewModel != null);
        }
    }
    
}
