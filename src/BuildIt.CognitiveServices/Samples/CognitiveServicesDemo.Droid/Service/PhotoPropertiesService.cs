using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CognitiveServicesDemo.Service.Interfaces;
using CognitiveServicesDrawRectangle.Service.Interface;
using PCLStorage;
using Xamarin.Forms;

namespace CognitiveServicesDemo.Droid.Service
{
    class PhotoPropertiesService : IPhotoPropertiesService
    {

        public async Task<string> PhotoImageUriAsync(string imageUri)
        {
            try
            {
                IFile imageFile = await FileSystem.Current.GetFileFromPathAsync(imageUri);
                var photoStream = imageFile.OpenAsync(FileAccess.Read).Result;

                

                var webImage = new Image();
                webImage.Aspect = Aspect.AspectFit;
                webImage.Source = ImageSource.FromUri(new Uri(imageUri));
                var img = new Image();
                img.Source = ImageSource.FromFile(imageUri);
                var aX = webImage.AnchorX;
                var aY = webImage.AnchorY;
                var X = webImage.X;
                var Y = webImage.Y;
                return null;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
    }
}