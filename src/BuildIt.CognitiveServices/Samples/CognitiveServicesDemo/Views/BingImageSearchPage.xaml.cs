using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CognitiveServicesDemo.Model;
using CognitiveServicesDemo.ViewModels;
using Xamarin.Forms;

namespace CognitiveServicesDemo.Views
{
    public partial class BingImageSearchPage : ContentPage
    {
        public BingImageSearchViewModel CurrentViewModel => BindingContext as BingImageSearchViewModel;
        public BingImageSearchPage()
        {
            InitializeComponent();
        }

        private async void Search_OnClicked(object sender, EventArgs e)
        {
            await CurrentViewModel.BingSearchRequestAsync(CurrentViewModel.InputText);
        }

        //private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        //{
        //    Value search = (Value)e.SelectedItem;
        //    Device.OpenUri(new Uri(search.url));
        //}
    }
}
