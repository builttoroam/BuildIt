using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("BuildIt")]
[assembly: ExportEffect(typeof(BuildIt.Forms.Controls.iOS.TouchEffect), "TouchEffect")]
#pragma warning disable SA1300 // Element must begin with upper-case letter - iOS platform
namespace BuildIt.Forms.Controls.iOS
#pragma warning restore SA1300 // Element must begin with upper-case letter
{
    /// <summary>
    /// Effect to detect touch input on any control
    /// </summary>
    public class TouchEffect : PlatformEffect
    {
        private UIView view;
        private TouchRecognizer touchRecognizer;

        /// <summary>
        /// Attach the effect
        /// </summary>
        protected override void OnAttached()
        {
            // Get the iOS UIView corresponding to the Element that the effect is attached to
            view = Control ?? Container;

            // Get access to the TouchEffect class in the PCL
            BuildIt.Forms.Controls.TouchEffect effect = (BuildIt.Forms.Controls.TouchEffect)Element.Effects.FirstOrDefault(e => e is BuildIt.Forms.Controls.TouchEffect);

            if (effect != null && view != null)
            {
                // Create a TouchRecognizer for this UIView
                touchRecognizer = new TouchRecognizer(Element, view, effect);
                view.AddGestureRecognizer(touchRecognizer);
            }
        }

        /// <summary>
        /// Detach the effect
        /// </summary>
        protected override void OnDetached()
        {
            if (touchRecognizer != null)
            {
                // Clean up the TouchRecognizer object
                touchRecognizer.Detach();

                // Remove the TouchRecognizer from the UIView
                view.RemoveGestureRecognizer(touchRecognizer);
            }
        }
    }
}