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
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task Animate(VisualElement visualElement)
        {
            if (visualElement == null)
            {
                return;
            }

            await visualElement.TranslateTo(TranslationX, TranslationY, (uint)Duration);
        }
    }
}