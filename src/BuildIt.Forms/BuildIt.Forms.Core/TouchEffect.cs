using Xamarin.Forms;

namespace BuildIt.Forms
{
    /// <summary>
    /// Touch effect to allow an entity to react to touch events.
    /// </summary>
    public class TouchEffect : RoutingEffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TouchEffect"/> class.
        /// </summary>
        public TouchEffect()
            : base("BuildIt.TouchEffect")
        {
        }

        /// <summary>
        /// The touch input event
        /// </summary>
        public event TouchActionEventHandler TouchAction;

        /// <summary>
        /// Gets or sets a value indicating whether the touch input has been captured.
        /// </summary>
        public bool Capture { get; set; }

        /// <summary>
        /// Raises the touch action event.
        /// </summary>
        /// <param name="element">The source element.</param>
        /// <param name="args">The touch arguments.</param>
        public void OnTouchAction(Element element, TouchActionEventArgs args)
        {
            TouchAction?.Invoke(element, args);
        }
    }
}