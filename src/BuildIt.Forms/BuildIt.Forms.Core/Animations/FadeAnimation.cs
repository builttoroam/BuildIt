using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    public class FadeAnimation : StateAnimation
    {
        public double Opacity { get; set; }
        public override Task Animate(VisualElement visualElement)
        {
            if (visualElement == null) return null;
            return visualElement.FadeTo(Opacity, (uint)Duration);
        }
    }
}