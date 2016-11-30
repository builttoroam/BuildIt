using System;
using CognitiveServicesDemo.Service.Interfaces;
using CognitiveServicesDemo.ViewModels;
using Xamarin.Forms;

namespace CognitiveServicesDemo.Views
{
    public partial class BingSpeechPage : ContentPage
    {
        public static IBingSpeech BingSpeech;
        public Editor MyEditor = new Editor();

        public delegate void Speech();

        public BingSpeechViewModel CurrentViewModel => BindingContext as BingSpeechViewModel;
        public BingSpeechPage()
        {
            InitializeComponent();
        }

        
        

        public void Button_OnClicked(object sender, EventArgs e)
        {
            //CurrentViewModel.BingSpeech.StartSpeech(r =>
            //{

            //    Editor.Text = r;
            //});

            App.Change.StartSpeech((b) => { Editor.Text = b; });
        }

        public static void Init(IBingSpeech bingSpeech)
        {
            BingSpeech = bingSpeech;
        }

    }
}
