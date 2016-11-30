//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using Xamarin.Forms;

//namespace CognitiveServicesDemo
//{
//    public partial class CustomResultLayout : ContentView
//    {
//        public CustomResultLayout()
//        {
//            InitializeComponent();
//        }

//        public void DrawRectangle(List<Rectangle> rectangle, string imageUri, double imageWidth, double imageHeight)
//        {

//            DisplayImage.Source = imageUri;

//            for (int i = 0; i < ResultRelativeLayout.Children.Count; i++)
//            {
//                if (i != 0)
//                {
//                    ResultRelativeLayout.Children.Remove(ResultRelativeLayout.Children[i]);
//                }

//            }
//            var frame = new Frame() { OutlineColor = Color.Red };

//            foreach (var face in rectangle)
//            {
//                ResultRelativeLayout.Children.Add(frame, Constraint.RelativeToView(DisplayImage, (ResultRelativeLayout, DisplayImage) => face.X / imageWidth * this.DisplayImage.Width), Constraint.RelativeToView(DisplayImage, (ResultRelativeLayout, DisplayImage) => face.Y / imageHeight * this.DisplayImage.Height), Constraint.RelativeToView(DisplayImage, (ResultRelativeLayout, DisplayImage) => face.Width / imageWidth * this.DisplayImage.Width), Constraint.RelativeToView(DisplayImage, (ResultRelativeLayout, DisplayImage) => face.Height / imageHeight * this.DisplayImage.Height));
//            }
//        }
//    }
//}
