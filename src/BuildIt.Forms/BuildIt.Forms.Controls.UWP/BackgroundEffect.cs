using System;
using System.Linq;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportEffect(typeof(BuildIt.Forms.Controls.UWP.BackgroundEffect), "BackgroundEffect")]

namespace BuildIt.Forms.Controls.UWP
{
    /// <summary>
    /// Effect for setting background on elements that don't support background
    /// </summary>
    [Preserve]
    public class BackgroundEffect : PlatformEffect
    {
        /// <summary>
        /// Attach the effect
        /// </summary>
        protected override void OnAttached()
        {
            // Get the Windows FrameworkElement corresponding to the Element that the effect is attached to
            var frameworkElement = Control ?? Container;

            // Get access to the TouchEffect class in the PCL
            var effect = (Forms.BackgroundEffect)Element.Effects.
                        FirstOrDefault(e => e is Forms.BackgroundEffect);

            var gd = frameworkElement as LayoutRenderer;
            if (gd == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(effect.Resource))
            {
                if (Windows.UI.Xaml.Application.Current.Resources[effect.Resource] is Brush brush)
                {
                    gd.Background = brush;
                }
            }

            if ((gd.Background as SolidColorBrush) == null ||
                (!(gd.Background as SolidColorBrush).Color.Equals(default(Color)) && effect.FallbackColor != default(Color)))
            {
                gd.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(
                    Convert.ToByte(effect.FallbackColor.A * 255),
                    Convert.ToByte(effect.FallbackColor.R * 255),
                    Convert.ToByte(effect.FallbackColor.G * 255),
                    Convert.ToByte(effect.FallbackColor.B * 255)));
            }
        }

        /// <summary>
        /// Detact the effect
        /// </summary>
        protected override void OnDetached()
        {
        }
    }
}