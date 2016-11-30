using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CognitiveServicesDemo.Model;
using CognitiveServicesDemo.ViewModels;
using Xamarin.Forms;

namespace CognitiveServicesDemo.Views
{
    public partial class EntityLinkingPage : ContentPage
    {
        public EntityLinkingViewModel CurrentViewModel => BindingContext as EntityLinkingViewModel;
        public EntityLinkingPage()
        {
            InitializeComponent();
        }

        private async void CheckEntityLinking_OnClicked(object sender, EventArgs e)
        {
            
            //OutputLabel.Text = null;
            CurrentViewModel.InputText = Editor.Text;
            await CurrentViewModel.EntityLinkingRequestAsync(CurrentViewModel.InputText);
            //OutputLabel.Text = CurrentViewModel.OutPutText;
            
        }

        private void EntityListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Entity entity = (Entity) e.SelectedItem;
            Device.OpenUri(new Uri($"https://en.wikipedia.org/wiki/{entity.wikipediaId}"));

        }
    }
}
