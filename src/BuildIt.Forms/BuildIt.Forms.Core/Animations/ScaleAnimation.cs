using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    /// <summary>
    /// Animation to scale an element
    /// </summary>
    public class ScaleAnimation : StateAnimation
    {
        /// <summary>
        /// Gets or sets the target scale of an element
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

            await visualElement.ScaleTo(Scale, (uint)Duration);
        }
    }
}