using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BuildIt.CognitiveServices.XForms.UserControls
{
    public partial class FacialRecognitionImage
    {
        private bool imageLoaded;

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
        public void DrawRectangle(List<Rectangle> rectangle, string imageUri, double imageWidth,
            double imageHeight)
        {
            DisplayImage.Source = imageUri;

            var existingFrames = ResultRelativeLayout.Children.OfType<Frame>().ToList();
            foreach (var existingFrame in existingFrames)
            {
                ResultRelativeLayout.Children.Remove(existingFrame);
            }


            var frame = new Frame { OutlineColor = Color.Red };

            foreach (var face in rectangle)
            {
                var xConstraint = Constraint.RelativeToView(DisplayImage,
                    (rl, v) => face.X / imageWidth * DisplayImage.Width);
                var yConstraint = Constraint.RelativeToView(DisplayImage,
                    (rl, v) => face.Y / imageHeight * DisplayImage.Height);
                var widthConstraint = Constraint.RelativeToView(DisplayImage,
                    (rl, v) => face.Width / imageWidth * DisplayImage.Width);
                var heightConstraint = Constraint.RelativeToView(DisplayImage,
                    (rl, v) => face.Height / imageHeight * DisplayImage.Height);

                ResultRelativeLayout.Children.Add(frame, xConstraint, yConstraint, widthConstraint, heightConstraint);
            }

        }
    }
}
