using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Core
{
    public class IncrementRotateAnimation : StateAnimation
    {
        public double Rotation { get; set; }
        public override Task Animate(VisualElement visualElement)
        {
            if (visualElement == null) return null;
            return visualElement.RelRotateTo(Rotation, (uint)Duration);
        }
    }
}