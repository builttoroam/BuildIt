using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    /// <summary>
    /// Animation to rotate an element by a specified amount
    /// </summary>
    public class IncrementRotateAnimation : StateAnimation
    {
        /// <summary>
        /// Gets or sets the amount to rotate the element
        /// </summary>
        public double Rotation { get; set; }

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

            await visualElement.RelRotateTo(Rotation, (uint)Duration);
        }
    }
}