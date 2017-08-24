using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(BuildIt.Forms.Controls.iOS.FontEffect), "FontEffect")]
#pragma warning disable SA1300 // Element must begin with upper-case letter - iOS platform
namespace BuildIt.Forms.Controls.iOS
#pragma warning restore SA1300 // Element must begin with upper-case letter
{
    /// <summary>
    /// Effect for specifying font
    /// </summary>
    public class FontEffect : PlatformEffect
    {
        /// <summary>
        /// Attach the effect
        /// </summary>
        protected override void OnAttached()
        {
            try
            {
                // Get access to the TouchEffect class in the PCL
                var effect = (Forms.FontEffect)Element.Effects.
                            FirstOrDefault(e => e is Forms.FontEffect);

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

        /// <summary>
        /// Detach the effect
        /// </summary>
        protected override void OnDetached()
        {
        }
    }
}