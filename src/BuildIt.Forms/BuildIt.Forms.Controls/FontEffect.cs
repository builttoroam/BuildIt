using Xamarin.Forms;

namespace BuildIt.Forms.Controls
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
    }
}
