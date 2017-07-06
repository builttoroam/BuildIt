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
        /// Gets the display name for the element
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Constructs the attribute and sets the name
        /// </summary>
        /// <param name="displayName"></param>
        public DisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}
