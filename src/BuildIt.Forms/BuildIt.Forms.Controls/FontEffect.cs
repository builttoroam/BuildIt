using Xamarin.Forms;

namespace BuildIt.Forms.Controls
{
    public class FontEffect : RoutingEffect
    {
        public string FontName { get; set; }
        public FontEffect() : base("BuildIt.FontEffect")
        {
        }
    }
   
}
