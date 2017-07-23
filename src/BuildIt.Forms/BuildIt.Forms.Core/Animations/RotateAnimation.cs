using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    public class RotateAnimation : StateAnimation
    {
        public double Rotation { get; set; }
        public override Task Animate(VisualElement visualElement)
        {
            if (visualElement == null) return null;
            return visualElement.RotateTo(Rotation, (uint)Duration);
        }
    }
}