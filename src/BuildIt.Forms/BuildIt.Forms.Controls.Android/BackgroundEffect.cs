using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(BuildIt.Forms.Controls.Droid.BackgroundEffect), "BackgroundEffect")]
namespace BuildIt.Forms.Controls.Droid
{
    public class BackgroundEffect : PlatformEffect
    {

        protected override void OnAttached()
        {
            // Get the Android View corresponding to the Element that the effect is attached to
            var view = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the PCL
            var effect =
                (Controls.BackgroundEffect)Element.Effects.
                    FirstOrDefault(e => e is Controls.BackgroundEffect);
            if (effect.FallbackColor != null)
            {
                view.SetBackgroundColor(new Android.Graphics.Color(
                        Convert.ToByte(effect.FallbackColor.R * 255),
                        Convert.ToByte(effect.FallbackColor.G * 255),
                        Convert.ToByte(effect.FallbackColor.B * 255),
                        Convert.ToByte(effect.FallbackColor.A * 255)));
            }

        }

        protected override void OnDetached()
        {
           
        }
    }
}

