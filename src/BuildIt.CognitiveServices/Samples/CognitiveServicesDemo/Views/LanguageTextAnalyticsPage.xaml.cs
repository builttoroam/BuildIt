using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CognitiveServicesDemo.ViewModels;
using Xamarin.Forms;

namespace CognitiveServicesDemo.Views
{
    public partial class LanguageTextAnalyticsPage : ContentPage
    {
        public LanguageTextAnalyticsViewModel CurrentViewModel => BindingContext as LanguageTextAnalyticsViewModel;

        public LanguageTextAnalyticsPage()
        {
            InitializeComponent();
        }

        private async void TextAnalytics_OnClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputEditor.Text) || string.IsNullOrWhiteSpace(InputEditor.Text))
            {
                CurrentViewModel.WarningText = "Please enter content e.g. I had a wonderful experience! The rooms were wonderful and the staff were helpful.";
            }
            else
            {
                await CurrentViewModel.TextAnalyticsAsync(InputEditor.Text);
            }
            
        }
    }
}
