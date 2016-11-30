using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CognitiveServicesDemo.ViewModels;
using Xamarin.Forms;

namespace CognitiveServicesDemo.Views
{
    public partial class LanguageBingSpellCheckPage : ContentPage
    {
        public LanguageBingSpellCheckViewModel CurrentViewModel => BindingContext as LanguageBingSpellCheckViewModel;

        public LanguageBingSpellCheckPage()
        {
            InitializeComponent();
            //LoadText();
        }

        private async void SpellCheck_OnClicked(object sender, EventArgs e)
        {
            //LoadText();
            if (string.IsNullOrEmpty(CurrentViewModel.InputText))
            {
                CurrentViewModel.Warning = "Please input e.g: A new service from micros oft!";
            }
            else
            {
                await CurrentViewModel.MakeSpellCheckRequestAsync(CurrentViewModel.InputText);
            }
        }
    }
}
