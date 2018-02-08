using System;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BuildIt.Forms
{
    /// <summary>
    /// An extension to load embedded images
    /// </summary>
    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        /// <summary>
        /// Gets or sets the path to the embedded image
        /// </summary>
        public string Source { get; set; }
                    
        /// <summary>
        /// Gets or sets holds a reference to the XAML control or page that is using this effect,
        /// so that the assembly it is defined on can be accessed to load the
        /// embedded font
        /// </summary>
        public object Parent { get; set; }

        /// <summary>
        /// Provides the image source
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <returns>The image source</returns>
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }

            try
            {
                // Do your translation lookup here, using whatever method you require
                var imageSource = ImageSource.FromResource(Source, Parent.GetType());

                return imageSource;
            }
            catch (Exception ex)
            {
                ex.LogError();
                return null;
            }
        }
    }
}