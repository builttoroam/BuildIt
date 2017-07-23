namespace BuildIt.Forms
{
    /// <summary>
    /// Represents a value that can be set on a target element
    /// </summary>
    public class Setter : TargettedStateAction
    {
        /// <summary>
        /// Gets or sets string representation of the value to set on the target
        /// </summary>
        public string Value { get; set; }
    }
}