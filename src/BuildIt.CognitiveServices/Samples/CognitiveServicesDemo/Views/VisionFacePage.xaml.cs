using System;
using System.Collections.Generic;
using System.Diagnostics;
using CognitiveServicesDemo.ViewModels;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Emotion;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace CognitiveServicesDemo.Views
{
    public partial class VisionFacePage : ContentPage
    {
        public VisionFaceViewModel CurrentViewModel => BindingContext as VisionFaceViewModel;
        private FaceServiceClient FaceServiceClient { get; set; }
        private EmotionServiceClient EmotionServiceClient { get; set; }

        public Xamarin.Forms.Rectangle Rectangle1 { get; set; }

        public VisionFacePage()
        {
            InitializeComponent();
            //TestImage.Source = ImageSource.FromUri(new Uri("http://www.faceaface-paris.com/wp-content/uploads/2015/07/carre_homme.jpg"));
        }


        private async void BrowseButton_Click(object sender, EventArgs e)
        {
            var frane = new Frame() { OutlineColor = Color.Red };
            //ResultLayout.Children.Add(frane, Constraint.RelativeToView(TestImage, (ResultLayout, TestImage) => this.TestImage.Width * 0.25), Constraint.RelativeToView(TestImage, (ResultLayout, TestImage) => this.TestImage.Height * 0.28), Constraint.RelativeToView(TestImage, (ResultLayout, TestImage) => this.TestImage.Width * .4), Constraint.RelativeToView(TestImage, (ResultLayout, TestImage) => this.TestImage.Height * .4));
            
            
            //await CurrentViewModel.VisionFaceCheckAsync();
        }

        private async void EmotionButton_Click(object sender, EventArgs e)
        {
            await CurrentViewModel.VisionEmotionAsync();
        }

        private async void ComputerVisionButton_Click(object sender, EventArgs e)
        {
           // await CurrentViewModel.VisionComputerVisionAsync();

            //string[] imageMetadata = CurrentViewModel.ImageMetadata.Split(',');
            //RectImg.WidthRequest = int.Parse(imageMetadata[0])/2;
            //RectImg.HeightRequest = int.Parse(imageMetadata[1])/2;

            //string[] faceMetadata = CurrentViewModel.FaceMetadata[0].Split(',');
            //RectImg.RectLeft = int.Parse(faceMetadata[0]);
            //RectImg.RectTop = int.Parse(faceMetadata[1]);
            //RectImg.RectRight = int.Parse(faceMetadata[2]);
            //RectImg.RectBottom = int.Parse(faceMetadata[3]);


            //RectImg.Faces = CurrentViewModel.FaceMetadata;

        }

        private async void TakePhoto_Click(object sender, EventArgs e)
        {
            try
            {
                //var file1 = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                //{
                //    AllowCropping = true,
                //    Directory = "Receipts",
                //    Name = $"{DateTime.UtcNow}.jpg"

                //});
                await CrossMedia.Current.Initialize();
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported) return;
                
                // Supply media options for saving our photo after it's taken.
                var mediaOptions = new StoreCameraMediaOptions
                {
                    Directory = "Receipts",
                    Name = $"{DateTime.UtcNow}.jpg"
                };

                // Take a photo of the business receipt.
                var file = await CrossMedia.Current.TakePhotoAsync(mediaOptions);
                CurrentViewModel.ImageUrl = file.Path;
                //Image.Source = CurrentViewModel.ImageUrl;



                await CurrentViewModel.VisionComputerVisionAsync(file);
                var faceMetaData = CurrentViewModel.Xywh;

                


                //TestImage.Source = CurrentViewModel.ImageUrl;



                List<Rectangle> face = new List<Rectangle>();
                Rectangle rectangle = new Rectangle();
                foreach (var s in faceMetaData)
                {
                    if (s != null)
                    {
                        string[] xywh = s.Split(',');
                        
                        rectangle.X = double.Parse(xywh[0]);
                        rectangle.Y = double.Parse(xywh[1]);
                        rectangle.Width = double.Parse(xywh[2]);
                        rectangle.Height = double.Parse(xywh[3]);
                        
                        face.Add(rectangle);


                        //ResultLayout.Children.Add(frane, Constraint.RelativeToView(TestImage, (ResultLayout, TestImage) => left/imageWidht*this.TestImage.Width), Constraint.RelativeToView(TestImage, (ResultLayout, TestImage) => top/imageHeight*this.TestImage.Height), Constraint.RelativeToView(TestImage, (ResultLayout, TestImage) => width/imageWidht* this.TestImage.Width), Constraint.RelativeToView(TestImage, (ResultLayout, TestImage) => height/imageHeight * this.TestImage.Height));

                    }
                }
                var imageMetadata = CurrentViewModel.ImageMetadata.Split(',');
                var imageWidht = int.Parse(imageMetadata[0]);
                var imageHeight = int.Parse(imageMetadata[1]);
                Debug.WriteLine($"current metadata\n{CurrentViewModel.ImageMetadata}");


                Layout.DrawRectangle(face, CurrentViewModel.ImageUrl, imageWidht,imageHeight);
                //string[] xywh = faceMetaData.Split(',');
                CurrentViewModel.WarningText = "Here is the computer vision results for you";
                //RectImage.Source = CurrentViewModel.ImageUrl;


                //CurrentViewModel.ImageUrl = file.Path;
                //await PclStorageSamle(file.Path);
            }
            catch (Exception ex)
            {
                // ignored
            }
        }
    }
}
