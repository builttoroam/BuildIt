using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Core
{
    public class ScaleAnimation : StateAnimation
    {
        public double Scale { get; set; }
        public override Task Animate(VisualElement visualElement)
        {
            if (visualElement == null) return null;
            return visualElement.ScaleTo(Scale, (uint)Duration);
        }
    }
}