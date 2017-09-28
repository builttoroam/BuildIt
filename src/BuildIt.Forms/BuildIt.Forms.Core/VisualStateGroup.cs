using BuildIt.States.Interfaces;
using System.Collections.Generic;

namespace BuildIt.Forms
{
    /// <summary>
    /// Represents a list of visual states
    /// </summary>
    public class VisualStateGroup : List<VisualState>
    {
        /// <summary>
        /// Gets or sets the corresponding state group
        /// </summary>
        public IStateGroup StateGroup { get; set; }

        /// <summary>
        /// Gets or sets the name of the visual state group
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the cache key for reusuing group definitions
        /// </summary>
        public string DefinitionCacheKey { get; set; }
    }
}