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
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task Animate(VisualElement visualElement)
        {
            if (visualElement == null)
            {
                return;
            }

            await visualElement.RelScaleTo(Scale, (uint)Duration);
        }
    }
}