using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    /// <summary>
    /// Animation to translate an element
    /// </summary>
    public class TranslateAnimation : StateAnimation
    {
        /// <summary>
        /// Gets or sets the target X translation
        /// </summary>
        public double TranslationX { get; set; }

        /// <summary>
        /// Gets or sets the target Y translation
        /// </summary>
        public double TranslationY { get; set; }

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

            await visualElement.TranslateTo(TranslationX, TranslationY, (uint)Duration);
        }
    }
}