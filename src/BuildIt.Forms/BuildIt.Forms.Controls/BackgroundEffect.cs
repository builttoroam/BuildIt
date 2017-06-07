using Xamarin.Forms;

namespace BuildIt.Forms.Controls
{
    public class BackgroundEffect : RoutingEffect
    {
        public string Resource { get; set; }

        public Color FallbackColor { get; set; }

        public BackgroundEffect() : base("BuildIt.BackgroundEffect")
        {
        }
              
    }
}
