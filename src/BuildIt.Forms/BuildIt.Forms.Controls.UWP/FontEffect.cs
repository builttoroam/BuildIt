using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportEffect(typeof(BuildIt.Forms.Controls.UWP.FontEffect), "FontEffect")]
namespace BuildIt.Forms.Controls.UWP
{
    /// <summary>
    /// Effect for specifying font on Label elements
    /// </summary>
    public class FontEffect : PlatformEffect
    {
        private FrameworkElement frameworkElement;
        private BuildIt.Forms.Controls.FontEffect effect;

        /// <summary>
        /// Attach the effect
        /// </summary>
        protected override void OnAttached()
        {
            try
            {
                // Get the Windows FrameworkElement corresponding to the Element that the effect is attached to
                frameworkElement = Control ?? Container;

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

        /// <summary>
        /// Detach the effect
        /// </summary>
        protected override void OnDetached()
        {
        }
    }
}
