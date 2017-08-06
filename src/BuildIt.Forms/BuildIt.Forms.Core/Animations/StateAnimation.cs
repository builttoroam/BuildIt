using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuildIt.Forms.Animations
{
    /// <summary>
    /// Base animation for an element
    /// </summary>
    public abstract class StateAnimation : TargettedStateAction
    {
        /// <summary>
        /// Animate method
        /// </summary>
        /// <param name="visualElement">The element to animate</param>
        /// <param name="cancelToken">Cancellation token so animation can be cancelled</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public abstract Task Animate(VisualElement visualElement, CancellationToken cancelToken);
    }
}