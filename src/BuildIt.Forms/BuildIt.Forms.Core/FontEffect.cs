using Xamarin.Forms;

namespace BuildIt.Forms
{
    /// <summary>
    /// Effect for setting font
    /// </summary>
    public class FontEffect : RoutingEffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontEffect"/> class.
        /// </summary>
        public FontEffect()
            : base("BuildIt.FontEffect")
        {
        }

        /// <summary>
        /// Gets or sets the font name eg fontawesome-webfont.ttf#FontAwesome
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the font is embedded in the current assembly
        /// </summary>
        public bool IsEmbedded { get; set; }

        public object Parent { get; set; }
    }
}
