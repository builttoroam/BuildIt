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
    public partial class SearchBingAutosuggestPage : ContentPage
    {
        public SearchBingAutosuggestViewModel CurrentViewModel => BindingContext as SearchBingAutosuggestViewModel;
        public SearchBingAutosuggestPage()
        {
            InitializeComponent();
        }

        private async void Search_OnClicked(object sender, EventArgs e)
        {

            await CurrentViewModel.BingAutoSuggestRequestAsync(CurrentViewModel.InputText);
        }

        private void AutosuggestPage_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            SearchSuggestion search = (SearchSuggestion) e.SelectedItem;
            Device.OpenUri(new Uri(search.url));
        }
    }
}
