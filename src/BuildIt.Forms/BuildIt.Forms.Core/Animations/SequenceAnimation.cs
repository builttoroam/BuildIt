using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    public class SequenceAnimation : MultiAnimation
    {
        public override async Task Animate(VisualElement visualElement)
        {
            if (visualElement == null) return;



            foreach (var anim in Animations)
            {
                var target = visualElement.FindByTarget(anim);
                var tg = target?.Item1 as VisualElement;
                await anim.Animate(tg ?? (visualElement as VisualElement));
            }
        }
    }
}