using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using UIKit;

[assembly: ResolutionGroupName("BuildIt")]
[assembly: ExportEffect(typeof(BuildIt.Forms.Controls.iOS.TouchEffect), "TouchEffect")]

namespace BuildIt.Forms.Controls.iOS
{
    public class TouchEffect : PlatformEffect
    {
        UIView view;
        TouchRecognizer touchRecognizer;

        protected override void OnAttached()
        {
            // Get the iOS UIView corresponding to the Element that the effect is attached to
            view = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the PCL
            BuildIt.Forms.Controls.TouchEffect effect = (BuildIt.Forms.Controls.TouchEffect)Element.Effects.FirstOrDefault(e => e is BuildIt.Forms.Controls.TouchEffect);

            if (effect != null && view != null)
            {
                // Create a TouchRecognizer for this UIView
                touchRecognizer = new TouchRecognizer(Element, view, effect); 
                view.AddGestureRecognizer(touchRecognizer);
            }
        }

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