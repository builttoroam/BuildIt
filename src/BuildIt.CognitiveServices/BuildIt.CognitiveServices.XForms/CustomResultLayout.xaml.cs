using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace BuildIt.CognitiveServices.XForms
{
    public partial class CustomResultLayout : ContentView
    {
        public CustomResultLayout()
        {
            InitializeComponent();
        }

        public void DrawRectangle(List<Rectangle> rectangle, string imageUri, double imageWidth, double imageHeight)
        {

            DisplayImage.Source = imageUri;

            var existingFrames = ResultRelativeLayout.Children.OfType<Frame>().ToList();
            foreach (var existingFrame in existingFrames)
            {
                ResultRelativeLayout.Children.Remove(existingFrame);
            }
            var frame = new Frame() { OutlineColor = Color.Red };

            foreach (var face in rectangle)
            {
                ResultRelativeLayout.Children.Add(frame, Constraint.RelativeToView(DisplayImage, (ResultRelativeLayout, DisplayImage) => face.X / imageWidth * this.DisplayImage.Width), Constraint.RelativeToView(DisplayImage, (ResultRelativeLayout, DisplayImage) => face.Y / imageHeight * this.DisplayImage.Height), Constraint.RelativeToView(DisplayImage, (ResultRelativeLayout, DisplayImage) => face.Width / imageWidth * this.DisplayImage.Width), Constraint.RelativeToView(DisplayImage, (ResultRelativeLayout, DisplayImage) => face.Height / imageHeight * this.DisplayImage.Height));
            }
        }

        private void DisplayImage_OnSizeChanged(object sender, EventArgs e)
        {
            Debug.WriteLine($"size changed {DisplayImage.Width} {DisplayImage.Height}" );
        }
    }
}
