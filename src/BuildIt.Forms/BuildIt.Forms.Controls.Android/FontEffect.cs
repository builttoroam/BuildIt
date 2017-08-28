using Android.Graphics;
using Android.Widget;
using Java.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private static IDictionary<string, Typeface> Fonts { get; } = new Dictionary<string, Typeface>();

        private string FileName { get; set; }

        private bool IsEmbedded { get; set; }

        private Assembly ParentAssembly { get; set; }

        /// <summary>
        /// Handle when the effect is added to an element
        /// </summary>
        protected override void OnAttached()
        {
            try
            {
                var view = Control ?? Container;

                var effect = (Forms.FontEffect)Element.Effects.
                         FirstOrDefault(e => e is Forms.FontEffect);
                FileName = effect?.FontName.Split('#').FirstOrDefault();
                if (string.IsNullOrWhiteSpace(FileName))
                {
                    return;
                }

                IsEmbedded = effect?.IsEmbedded ?? false;
                if (IsEmbedded)
                {
                    ParentAssembly = effect?.Parent.GetType().Assembly;
                }

                ApplyToLabels(view as TextView);
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
        }

        /// <summary>
        /// Detach effect
        /// </summary>
        protected override void OnDetached()
        {
        }

        private void ApplyToLabels(TextView view)
        {
            if (view == null)
            {
                return;
            }

            var font = Fonts.SafeValue(FileName);
            if (font == null)
            {
                if (!IsEmbedded)
                {
                    font = Typeface.CreateFromAsset(Xamarin.Forms.Forms.Context.Assets, "Fonts/" + FileName);
                }
                else
                {
                    var fontFile = ExtractFont(FileName);
                    if (fontFile == null)
                    {
                        return;
                    }

                    font = Typeface.CreateFromFile(fontFile);
                }

                Fonts[FileName] = font;
            }

            view.Typeface = font;
        }

        private File ExtractFont(string fileName)
        {
            try
            {
                var assembly = ParentAssembly;
                var file = File.CreateTempFile("__fonteffect", ".ttf");
                var resourceName = assembly.FullName.Split(',').FirstOrDefault() + "." + fileName;
                $"Attempting to open resource {resourceName}".Log();
                var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    "Resource doesn't exist".Log();
                    return null;
                }

                using (var fs = System.IO.File.OpenWrite(file.Path))
                {
                    stream.CopyTo(fs);
                    fs.Flush(true);
                }

                "Font successfully extracted".Log();
                return file;
            }
            catch (Exception ex)
            {
                $"Unable to extract font file '{fileName}'".Log();
                ex.LogException();
                return null;
            }
        }
    }
}
