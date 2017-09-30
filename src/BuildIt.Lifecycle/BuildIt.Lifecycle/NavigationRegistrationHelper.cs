using System;

namespace BuildIt.Lifecycle
{
    /// <summary>
    /// Entity to assist with the registration of views - holds
    /// a reference to the type of view that's registered
    /// </summary>
    public class NavigationRegistrationHelper
    {
        /// <summary>
        /// Gets or sets type of view that's being registered
        /// </summary>
        public Type ViewType { get; set; }
    }
}