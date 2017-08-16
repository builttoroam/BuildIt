using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    /// <summary>
    /// A list of animations that will be invoked in sequence
    /// </summary>
    public class SequenceAnimation : MultiAnimation
    {
        /// <summary>
        /// Animate method
        /// </summary>
        /// <param name="visualElement">The element to animate</param>
        /// <param name="cancelToken">Cancellation token to cancel animation</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task Animate(VisualElement visualElement, CancellationToken cancelToken)
        {
            if (visualElement == null)
            {
                return;
            }

            foreach (var anim in Animations)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }
                var target = visualElement.FindByTarget(anim);
                var tg = target?.Item1 as VisualElement;
                await anim.Animate(tg ?? visualElement, cancelToken);
            }
        }
    }
}