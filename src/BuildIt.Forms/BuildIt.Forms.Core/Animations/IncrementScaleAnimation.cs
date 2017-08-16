using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    /// <summary>
    /// Animation for scaling an element incrementally (not a target scale)
    /// </summary>
    public class IncrementScaleAnimation : StateAnimation
    {
        /// <summary>
        /// Gets or sets the amount to incrementally scale the element by
        /// </summary>
        public double Scale { get; set; }

        /// <summary>
        /// Animate method
        /// </summary>
        /// <param name="visualElement">The element to animate</param>
        /// <param name="cancelToken">Cancellation token so animation can be cancelled</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task Animate(VisualElement visualElement, CancellationToken cancelToken)
        {
            if (visualElement == null)
            {
                return;
            }

            cancelToken.Register(() => ViewExtensions.CancelAnimations(visualElement));

            await visualElement.RelScaleTo(Scale, (uint)Duration);
        }
    }
}