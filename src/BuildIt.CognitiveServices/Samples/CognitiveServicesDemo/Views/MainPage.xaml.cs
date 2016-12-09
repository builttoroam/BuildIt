using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CognitiveServicesDemo.ViewModels;
using Xamarin.Forms;

namespace CognitiveServicesDemo.Views
{
    public partial class MainPage : ContentPage
    {
        public MainViewModel CurrentViewModel => BindingContext as MainViewModel;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void VisionEmotions(object sender, EventArgs e)
        {
            CurrentViewModel.ShowEmotion();
        }

        private async void VisionFace(object sender, EventArgs e)
        {
            CurrentViewModel.ShowFace();
        }

        private async void LanguageBingSpell(object sender, EventArgs e)
        {
            CurrentViewModel.ShowBingSpellCheck();
        }

        private async void LanguageTextAnalytics(object sender, EventArgs e)
        {
            CurrentViewModel.ShowTextAnalytics();
        }

        private void LanguageWlm(object sender, EventArgs e)
        {
            CurrentViewModel.ShowWlm();
        }

        private void EntityLinking(object sender, EventArgs e)
        {
            CurrentViewModel.ShowEntityLinking();
        }

        private void BingAutosuggest(object sender, EventArgs e)
        {
            CurrentViewModel.ShowBingAutosuggest();
        }

        private void BingWebSearch(object sender, EventArgs e)
        {
            CurrentViewModel.ShowBingWebSearch();
        }

        private void BingSpeech(object sender, EventArgs e)
        {
            CurrentViewModel.ShowBingSpeech();
        }

        private void BingImageSearch(object sender, EventArgs e)
        {
            CurrentViewModel.ShowBingImageSearch();
        }

        private void Academic(object sender, EventArgs e)
        {
            CurrentViewModel.ShowAcademic();
        }

        private void VideoFacialRecognition(object sender, EventArgs e)
        {
            CurrentViewModel.ShowVideo();
        }
    }
}
