using System;
using Xamarin.Forms.Xaml;

namespace BuildIt.Forms.Sample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SavedPhotoPage
    {
        public SavedPhotoPage(string photoFilePath)
        {
            InitializeComponent();

            // NB: In UWP this willy only work if the image source is in the app's local storage directory. Pictures Library sources do not work :( -RR
            Image.Source = photoFilePath;
        }

        private async void CloseButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(true);
        }
    }
}