using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    /// <summary>
    /// Animatino that rotates an element to a specified rotation
    /// </summary>
    public class RotateAnimation : StateAnimation
    {
        /// <summary>
        /// Gets or sets the target rotation
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

            await visualElement.RotateTo(Rotation, (uint)Duration);
        }
    }
}