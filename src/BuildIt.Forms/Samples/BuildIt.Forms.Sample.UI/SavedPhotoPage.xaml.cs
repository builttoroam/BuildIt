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
            Image.Source = photoFilePath;
        }

        private async void CloseButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(true);
        }
    }
}