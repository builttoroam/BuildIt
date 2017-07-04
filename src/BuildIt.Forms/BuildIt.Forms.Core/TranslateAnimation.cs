using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Core
{
    public class TranslateAnimation : StateAnimation
    {
        public double TranslationX { get; set; }
        public double TranslationY { get; set; }
        public override Task Animate(VisualElement visualElement)
        {
            if (visualElement == null) return null;
            return visualElement.TranslateTo(TranslationX, TranslationY, (uint)Duration);
        }
    }
}