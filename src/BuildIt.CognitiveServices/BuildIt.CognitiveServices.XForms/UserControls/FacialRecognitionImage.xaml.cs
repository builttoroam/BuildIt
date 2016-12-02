using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BuildIt.CognitiveServices.XForms.UserControls
{
    public partial class FacialRecognitionImage
    {
        public FacialRecognitionImage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Draw rectangle on face
        /// </summary>
        /// <param name="rectangle">
        /// X, Y width and hight value of face
        /// </param>
        /// <param name="imageUri">
        /// Pass imageUri to display image
        /// </param>
        /// <param name="imageWidth">
        /// Width of image
        /// </param>
        /// <param name="imageHeight">
        /// Height of image
        /// </param>
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
    }
}
