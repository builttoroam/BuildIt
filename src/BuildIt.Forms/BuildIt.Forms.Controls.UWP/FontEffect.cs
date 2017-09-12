using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportEffect(typeof(BuildIt.Forms.Controls.UWP.FontEffect), "FontEffect")]
namespace BuildIt.Forms.Controls.UWP
{
    /// <summary>
    /// Effect for specifying font on Label elements
    /// </summary>
    [Preserve]
    public class FontEffect : PlatformEffect
    {
        private static IDictionary<string, FontFamily> Fonts { get; } = new Dictionary<string, FontFamily>();

        private FrameworkElement frameworkElement;
        private Forms.FontEffect effect;

        private bool IsEmbedded { get; set; }

        private Assembly ParentAssembly { get; set; }

        /// <summary>
        /// Attach the effect
        /// </summary>
        protected override async void OnAttached()
        {
            try
            {
                // Get the Windows FrameworkElement corresponding to the Element that the effect is attached to
                frameworkElement = Control ?? Container;

                // Get access to the TouchEffect class in the PCL
                effect = (Forms.FontEffect)Element.Effects.
                            FirstOrDefault(e => e is Forms.FontEffect);
                var parts = effect?.FontName.Split('#');
                var fileName = parts.FirstOrDefault();
                var familyName = parts.LastOrDefault();
                IsEmbedded = effect?.IsEmbedded ?? false;
                if (IsEmbedded)
                {
                    ParentAssembly = effect?.Parent.GetType().GetTypeInfo().Assembly;
                }


                if (effect != null && frameworkElement != null)
                {
                    if (IsEmbedded)
                    {
                        var font = Fonts.SafeValue(effect.FontName);
                        if (font == null)
                        {
                            var file = await ExtractFont(fileName);
                            var uri = new Uri("ms-appdata:///local/" + file.Name);
                            font = new FontFamily(uri.OriginalString +"#" + familyName);
                            Fonts[effect.FontName] = font;
                        }
                        (frameworkElement as TextBlock).FontFamily = font;
                    }
                    else
                    {

                        (this.Element as Label).FontFamily = "/Assets/Fonts/" + effect.FontName;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        private async Task<StorageFile> ExtractFont(string fileName)
        {
            try
            {
                var assembly = ParentAssembly;
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("__fonteffect" + fileName, CreationCollisionOption.ReplaceExisting);
                var resourceName = assembly.FullName.Split(',').FirstOrDefault() + "." + fileName;
                $"Attempting to open resource {resourceName}".Log();
                var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    "Resource doesn't exist".Log();
                    return null;
                }

                using (var fs = await file.OpenStreamForWriteAsync())
                {
                    await stream.CopyToAsync(fs);
                    await fs.FlushAsync();
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

        /// <summary>
        /// Detach the effect
        /// </summary>
        protected override void OnDetached()
        {
        }
    }
}
