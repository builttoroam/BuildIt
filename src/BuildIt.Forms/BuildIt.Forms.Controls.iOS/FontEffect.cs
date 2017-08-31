using System;
using System.Linq;
using System.Reflection;
using CoreGraphics;
using CoreText;
using Foundation;
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
        private bool IsEmbedded { get; set; }

        private Assembly ParentAssembly { get; set; }

        /// <summary>
        /// Attach the effect
        /// </summary>
        protected override void OnAttached()
        {
            try
            {
                var label = Element as Label;
                if (label == null)
                {
                    return;
                }

                // Get access to the TouchEffect class in the PCL
                var effect = (Forms.FontEffect)Element.Effects.
                            FirstOrDefault(e => e is Forms.FontEffect);

                IsEmbedded = effect?.IsEmbedded ?? false;
                if (IsEmbedded)
                {
                    ParentAssembly = effect?.Parent.GetType().Assembly;
                }

                var pieces = effect.FontName?.Split('#');
                if (IsEmbedded)
                {
                    var fileName = pieces?.FirstOrDefault();
                    var fontOk = ExtractFont(fileName);
                    if (!fontOk)
                    {
                        return;
                    }
                }

                label.FontFamily = pieces?.LastOrDefault();

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

        private bool ExtractFont(string fileName)
        {
            try
            {
                var assembly = ParentAssembly;
                var resourceName = assembly.FullName.Split(',').FirstOrDefault() + "." + fileName;
                $"Attempting to open resource {resourceName}".Log();
                var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    "Resource doesn't exist".Log();
                }

                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                var fontData = NSData.FromArray(data);
                using (var provider = new CGDataProvider(fontData))
                using (var nativeFont = CGFont.CreateFromProvider(provider))
                {
                    NSError error;
                    CTFontManager.RegisterGraphicsFont(nativeFont, out error);
                    if (error != null)
                    {
                        Console.WriteLine(error);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ex.LogException();
                return false;
            }
        }
    }
}