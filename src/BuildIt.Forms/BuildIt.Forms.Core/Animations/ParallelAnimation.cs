using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    /// <summary>
    /// Represents a list of animations that will be run in parallel
    /// </summary>
    public class ParallelAnimation : MultiAnimation
    {
        /// <summary>
        /// Animate method
        /// </summary>
        /// <param name="visualElement">The element to animate</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task Animate(VisualElement visualElement)
        {
            if (visualElement == null)
            {
                return;
            }

            var tasks = from anim in Animations
                        let target = visualElement.FindByTarget(anim)
                        let tg = target?.Item1 as VisualElement
                        select anim.Animate(tg ?? visualElement);
            await Task.WhenAll(tasks);
        }
    }
}