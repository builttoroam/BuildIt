using Android.Graphics;
using Android.Views;
using Android.Widget;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(BuildIt.Forms.Controls.Droid.FontEffect), nameof(BuildIt.Forms.Controls.Droid.FontEffect))]
// ReSharper disable once CheckNamespace - Needs to be platform specific
namespace BuildIt.Forms.Controls.Droid
{
    /// <summary>
    /// Font effect, used to specify font family for visual elements
    /// </summary>
    public class FontEffect : PlatformEffect
    {
        private string FileName { get; set; }

        /// <summary>
        /// Handle when the effect is added to an element
        /// </summary>
        protected override void OnAttached()
        {
            try
            {
                var view = Control ?? Container;

                var effect = (BuildIt.Forms.Controls.FontEffect)Element.Effects.
                         FirstOrDefault(e => e is BuildIt.Forms.Controls.FontEffect);
                FileName = effect?.FontName.Split('#').FirstOrDefault();
                if (string.IsNullOrWhiteSpace(FileName))
                {
                    return;
                }

                ApplyToLabels(view as ViewGroup);

                // control = Control as TextView;
                // Typeface font = Typeface.CreateFromAsset(Xamarin.Forms.Forms.Context.ApplicationContext.Assets, "Fonts/" + fileName);
                // control.Typeface = font;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        /// <summary>
        /// Detach effect
        /// </summary>
        protected override void OnDetached()
        {
        }

        private void ApplyToLabels(ViewGroup group)
        {
            if (group == null)
            {
                return;
            }

            var cnt = group.ChildCount;
            for (int i = 0; i < cnt; i++)
            {
                var child = group.GetChildAt(i);
                if (child is TextView control)
                {
                    Typeface font = Typeface.CreateFromAsset(Xamarin.Forms.Forms.Context.Assets, "Fonts/" + FileName);
                    control.Typeface = font;
                }
                else if (child is ViewGroup subgroup)
                {
                    ApplyToLabels(subgroup);
                }
            }
        }
    }
}
