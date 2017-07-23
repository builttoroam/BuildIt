using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    public class IncrementScaleAnimation : StateAnimation
    {
        public double Scale { get; set; }
        public override Task Animate(VisualElement visualElement)
        {
            if (visualElement == null) return null;
            return visualElement.RelScaleTo(Scale, (uint)Duration);
        }
    }
}