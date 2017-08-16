using System.Linq;
using BuildIt.Forms.Controls.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(BackgroundEffect), nameof(BackgroundEffect))]

#pragma warning disable SA1300 // Element must begin with upper-case letter - iOS platform
namespace BuildIt.Forms.Controls.iOS
#pragma warning restore SA1300 // Element must begin with upper-case letter
{
    /// <summary>
    /// Used to specify the background color (on elements that don't have a BackgroundColor
    /// attribute exposed on them). Can specify either/or a resource (eg XAML resource name) or
    /// fallback color.
    /// </summary>
    public class BackgroundEffect : PlatformEffect
    {
        private UIView view;

        /// <summary>
        /// Attach the effect
        /// </summary>
        protected override void OnAttached()
        {
            // Get the iOS UIView corresponding to the Element that the effect is attached to
            view = Control ?? Container;

            // Get access to the TouchEffect class in the PCL
            var effect = (Controls.BackgroundEffect)Element.Effects.FirstOrDefault(e => e is Controls.BackgroundEffect);

            if (effect?.FallbackColor != null && view != null)
            {
                view.BackgroundColor = new UIColor(
                    (float)effect.FallbackColor.R,
                    (float)effect.FallbackColor.G,
                    (float)effect.FallbackColor.B,
                    (float)effect.FallbackColor.A);
            }
        }

        /// <summary>
        /// Detach the effect
        /// </summary>
        protected override void OnDetached()
        {
        }
    }
}
