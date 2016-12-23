using BuildIt.CognitiveServices.Models;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BuildIt.CognitiveServices.XForms.UserControls
{
    public partial class FacialRecognitionImage
    {
        public static readonly BindableProperty FaceRectanglesProperty =
            BindableProperty.Create("FaceRectangles", typeof(IEnumerable<FaceRectangle>), typeof(FacialRecognitionImage), null, BindingMode.Default, null, FaceRectanglesUpdated);

        public static readonly BindableProperty ImageSourceProperty =
            BindableProperty.Create("ImageSource", typeof(ImageSource), typeof(FacialRecognitionImage));

        public static readonly BindableProperty NaturalImageWidthProperty =
            BindableProperty.Create("NaturalImageWidth", typeof(double), typeof(FacialRecognitionImage), 0.0d);

        public static readonly BindableProperty NaturalImageHeightProperty =
            BindableProperty.Create("NaturalImageHeight", typeof(double), typeof(FacialRecognitionImage), 0.0d);

        public FacialRecognitionImage()
        {
            InitializeComponent();
        }

        private static void FaceRectanglesUpdated(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as FacialRecognitionImage;
            if (control?.FaceRectangles != null)
            {
                control.DrawRectangles();
            }
        }

        public IEnumerable<FaceRectangle> FaceRectangles // Todo - make a less Xamarin Forms entity type here
        {
            get { return (IEnumerable<FaceRectangle>)GetValue(FaceRectanglesProperty); }
            set { SetValue(FaceRectanglesProperty, value); }
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public double NaturalImageWidth
        {
            get { return (double)GetValue(NaturalImageWidthProperty); }
            set { SetValue(NaturalImageWidthProperty, value); }
        }

        public double NaturalImageHeight
        {
            get { return (double)GetValue(NaturalImageHeightProperty); }
            set { SetValue(NaturalImageHeightProperty, value); }
        }

        private void DrawRectangles()
        {
            var existingFrames = ResultRelativeLayout.Children.OfType<Frame>().ToList();
            foreach (var existingFrame in existingFrames)
            {
                ResultRelativeLayout.Children.Remove(existingFrame);
            }

            foreach (var faceRectangle in FaceRectangles)
            {
                var frame = new Frame { OutlineColor = Color.Red, BackgroundColor = Color.Transparent };

                var xConstraint = Constraint.RelativeToView(Image,
                    (rl, v) => faceRectangle.X / NaturalImageWidth * v.Width);
                var yConstraint = Constraint.RelativeToView(Image,
                    (rl, v) => faceRectangle.Y / NaturalImageHeight * v.Height);
                var widthConstraint = Constraint.RelativeToView(Image,
                    (rl, v) => faceRectangle.Width / NaturalImageWidth * v.Width);
                var heightConstraint = Constraint.RelativeToView(Image,
                    (rl, v) => faceRectangle.Height / NaturalImageHeight * v.Height);

                ResultRelativeLayout.Children.Add(frame, xConstraint, yConstraint, widthConstraint, heightConstraint);
            }
        }
    }
}
