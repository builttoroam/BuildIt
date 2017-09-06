using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(BuildIt.Forms.Controls.Droid.BackgroundEffect), "BackgroundEffect")]
// ReSharper disable once CheckNamespace - keeping namespace different to make it easier to identify platform classes
namespace BuildIt.Forms.Controls.Droid
{
    /// <summary>
    /// Background effect used to set the background to Android views
    /// </summary>
    [Preserve]
    public class BackgroundEffect : PlatformEffect
    {
        /// <summary>
        /// Attaching the background effect
        /// </summary>
        protected override void OnAttached()
        {
            // Get the Android View corresponding to the Element that the effect is attached to
            var view = Control ?? Container;

            // Get access to the TouchEffect class in the PCL
            var effect = (Forms.BackgroundEffect)Element.Effects.FirstOrDefault(e => e is Forms.BackgroundEffect);
            if (effect?.FallbackColor != null)
            {
                view.SetBackgroundColor(new Android.Graphics.Color(
                        Convert.ToByte(effect.FallbackColor.R * 255),
                        Convert.ToByte(effect.FallbackColor.G * 255),
                        Convert.ToByte(effect.FallbackColor.B * 255),
                        Convert.ToByte(effect.FallbackColor.A * 255)));
            }
        }

        /// <summary>
        /// Detaches the effects
        /// </summary>
        protected override void OnDetached()
        {
        }
    }
}
