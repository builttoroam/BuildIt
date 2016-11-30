using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CognitiveServicesDemo.ViewModels;
using Xamarin.Forms;

namespace CognitiveServicesDemo.Views
{
    public partial class AcademicPage : ContentPage
    {
        public AcademicViewModel CurrentViewModel => BindingContext as AcademicViewModel;
        public AcademicPage()
        {
            InitializeComponent();
        }

        private async void Check_OnClicked(object sender, EventArgs e)
        {
            await CurrentViewModel.AcademicSearchRequestAsync(CurrentViewModel.InputText);
        }
    }
}
