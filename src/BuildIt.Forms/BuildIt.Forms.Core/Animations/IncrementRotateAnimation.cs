using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    /// <summary>
    /// Animation to rotate an element by a specified amount.
    /// </summary>
    public class IncrementRotateAnimation : StateAnimation
    {
        /// <summary>
        /// Gets or sets the amount to rotate the element.
        /// </summary>
        public double Rotation { get; set; }

        /// <summary>
        /// Animate method.
        /// </summary>
        /// <param name="visualElement">The element to animate.</param>
        /// <param name="cancelToken">Cancellation token so animation can be cancelled.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task Animate(VisualElement visualElement, CancellationToken cancelToken)
        {
            if (visualElement == null)
            {
                return;
            }

            cancelToken.Register(() => ViewExtensions.CancelAnimations(visualElement));

            await visualElement.RelRotateTo(Rotation, (uint)Duration);
        }
    }
}