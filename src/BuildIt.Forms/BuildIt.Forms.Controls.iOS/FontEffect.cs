using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(BuildIt.Forms.Controls.iOS.FontEffect), "FontEffect")]
namespace BuildIt.Forms.Controls.iOS
{
    public class FontEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                // Get access to the TouchEffect class in the PCL
                var effect = (BuildIt.Forms.Controls.FontEffect)Element.Effects.
                            FirstOrDefault(e => e is BuildIt.Forms.Controls.FontEffect);

                var pieces = effect.FontName?.Split('#');
                if ((pieces?.Length ?? 0) > 0)
                {
                    (this.Element as Label).FontFamily = pieces.LastOrDefault();
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