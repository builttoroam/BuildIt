using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Core
{
    public class RotateYAnimation : StateAnimation
    {
        public double Rotation { get; set; }
        public override Task Animate(VisualElement visualElement)
        {
            if (visualElement == null) return null;
            return visualElement.RotateYTo(Rotation, (uint)Duration);
        }
    }
}