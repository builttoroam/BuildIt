using BuildIt.ML.Sample.Core;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BuildIt.ML.Sample.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }

        private async void ClassifyButton_Clicked(object sender, EventArgs e)
        {
            var mainViewModel = BindingContext as MainViewModel;
            await mainViewModel.ClassifyAsync();
        }
    }
}