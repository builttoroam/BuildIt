using System;

namespace BuildIt
{
    /// <summary>
    /// Sets a display name for the element (enum or fields)
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class DisplayNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayNameAttribute"/> class.
        /// Constructs the attribute and sets the name
        /// </summary>
        /// <param name="displayName">The name to assign to the element</param>
        public DisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }

        /// <summary>
        /// Gets the display name for the element
        /// </summary>
        public string DisplayName { get; }
    }
}
