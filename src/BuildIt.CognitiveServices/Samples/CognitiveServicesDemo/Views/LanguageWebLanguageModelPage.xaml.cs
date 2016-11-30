using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CognitiveServicesDemo.ViewModels;
using Xamarin.Forms;

namespace CognitiveServicesDemo.Views
{
    public partial class LanguageWebLanguageModelPage : ContentPage
    {
        public LanguageWebLanguageModelViewModel CurrentViewModel => BindingContext as LanguageWebLanguageModelViewModel;
        public LanguageWebLanguageModelPage()
        {
            InitializeComponent();
        }

        private async void Analyse_OnClicked(object sender, EventArgs e)
        {
            await CurrentViewModel.BreakIntoWordRequestAsync(InputEditor.Text);
        }
    }
}
