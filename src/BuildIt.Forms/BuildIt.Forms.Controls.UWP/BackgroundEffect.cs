using System;
using System.Linq;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportEffect(typeof(BuildIt.Forms.Controls.UWP.BackgroundEffect), "BackgroundEffect")]
namespace BuildIt.Forms.Controls.UWP
{
    public class BackgroundEffect : PlatformEffect
    {
        
        protected override void OnAttached()
        {
            // Get the Windows FrameworkElement corresponding to the Element that the effect is attached to
            var frameworkElement = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the PCL
            var effect = (Controls.BackgroundEffect)Element.Effects.
                        FirstOrDefault(e => e is Controls.BackgroundEffect);

            var gd = frameworkElement as LayoutRenderer;
            if(gd!=null)
            {
                if (!string.IsNullOrEmpty(effect.Resource))
                {
                    var brush = Windows.UI.Xaml.Application.Current.Resources[effect.Resource] as Brush;
                    if (brush != null)
                    {
                        gd.Background = brush;
                    }
                }
                
                if (gd.Background==null && effect.FallbackColor != default(Color))
                {
                    gd.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(
                        Convert.ToByte(effect.FallbackColor.A * 255),
                        Convert.ToByte(effect.FallbackColor.R * 255),
                        Convert.ToByte(effect.FallbackColor.G * 255),
                        Convert.ToByte(effect.FallbackColor.B * 255)));
                }
            }
        }

        protected override void OnDetached()
        {
            
        }
    }
}



