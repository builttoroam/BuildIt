using Xamarin.Forms;

namespace BuildIt.Forms.Controls
{
    /// <summary>
    /// Platform agnostic background effect
    /// </summary>
    public class BackgroundEffect
        : RoutingEffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundEffect"/> class.
        /// </summary>
        public BackgroundEffect()
            : base("BuildIt.BackgroundEffect")
        {
        }

        /// <summary>
        /// Gets or sets the resource (XAML) to use for the color
        /// </summary>
        public string Resource { get; set; }

        /// <summary>
        /// Gets or sets the fallback color
        /// </summary>
        public Color FallbackColor { get; set; }
    }
}
