using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    /// <summary>
    /// Animation to fade content by adjusting Opacity attribute
    /// </summary>
    public class FadeAnimation : StateAnimation
    {
        /// <summary>
        /// Gets or sets the target Opacity value
        /// </summary>
        public double Opacity { get; set; }

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

            await visualElement.FadeTo(Opacity, (uint)Duration);
        }
    }
}