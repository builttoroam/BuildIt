using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    /// <summary>
    /// RotateY animation.
    /// </summary>
    public class RotateYAnimation : StateAnimation
    {
        /// <summary>
        /// Gets or sets the target rotation.
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

            await visualElement.RotateYTo(Rotation, (uint)Duration);
        }
    }
}