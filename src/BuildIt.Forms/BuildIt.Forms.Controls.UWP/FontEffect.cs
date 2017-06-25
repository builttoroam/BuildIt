using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportEffect(typeof(BuildIt.Forms.Controls.UWP.FontEffect), "FontEffect")]
namespace BuildIt.Forms.Controls.UWP
{
    public class FontEffect : PlatformEffect
    {
        FrameworkElement frameworkElement;
        BuildIt.Forms.Controls.FontEffect effect;


        protected override void OnAttached()
        {
            try
            {
                // Get the Windows FrameworkElement corresponding to the Element that the effect is attached to
                frameworkElement = Control == null ? Container : Control;

                // Get access to the TouchEffect class in the PCL
                effect = (BuildIt.Forms.Controls.FontEffect)Element.Effects.
                            FirstOrDefault(e => e is BuildIt.Forms.Controls.FontEffect);

                if (effect != null && frameworkElement != null)
                {
                    (this.Element as Label).FontFamily = "/Assets/Fonts/" + effect.FontName;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        protected override void OnDetached()
        {
        }
    }
}

