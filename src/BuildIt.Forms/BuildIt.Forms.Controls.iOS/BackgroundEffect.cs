using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using UIKit;

[assembly: ExportEffect(typeof(BuildIt.Forms.Controls.iOS.BackgroundEffect), "BackgroundEffect")]

namespace BuildIt.Forms.Controls.iOS
{
    public class BackgroundEffect : PlatformEffect
    {
        UIView view;
        TouchRecognizer touchRecognizer;

        protected override void OnAttached()
        {
            // Get the iOS UIView corresponding to the Element that the effect is attached to
            view = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the PCL
           var effect = (Controls.BackgroundEffect)Element.Effects.FirstOrDefault(e => e is Controls.BackgroundEffect);

            if (effect != null && view != null)
            {
                if (effect.FallbackColor != null) {
                    view.BackgroundColor = new UIColor(
                        (float)effect.FallbackColor.R, 
                        (float)effect.FallbackColor.G,
                        (float)effect.FallbackColor.B,
                        (float)effect.FallbackColor.A);
                        }
            }
        }

        protected override void OnDetached()
        {
        }
    }
}


