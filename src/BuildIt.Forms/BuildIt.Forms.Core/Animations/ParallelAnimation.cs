using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    public class ParallelAnimation : MultiAnimation
    {
        public override Task Animate(VisualElement visualElement)
        {
            if (visualElement == null) return null;


            var tasks = (from anim in Animations
                let target = visualElement.FindByTarget(anim)
                let tg = target?.Item1 as VisualElement
                select anim.Animate(tg ?? (visualElement as VisualElement)));
            return Task.WhenAll((IEnumerable<Task>) tasks);
        }
    }
}