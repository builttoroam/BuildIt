using Xamarin.Forms;

namespace BuildIt.Forms
{
    /// <summary>
    /// A target action for a state (should be sub-classes - see Setter)
    /// </summary>
    public class TargettedStateAction
    {
        /// <summary>
        /// Gets or sets the target reference as string in format ElementName.Property
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Gets or sets the target element
        /// </summary>
        public Element Element { get; set; }

        /// <summary>
        /// Gets or sets the target property
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// Gets or sets the duration for the action
        /// </summary>
        public int Duration { get; set; }
    }
}