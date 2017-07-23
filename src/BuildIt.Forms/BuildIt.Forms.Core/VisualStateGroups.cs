using System.Collections.Generic;
using BuildIt.States;
using BuildIt.States.Interfaces;

namespace BuildIt.Forms
{
    /// <summary>
    /// Group of visual states in xaml
    /// </summary>
    public class VisualStateGroups : List<VisualStateGroup>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualStateGroups"/> class.
        /// </summary>
        public VisualStateGroups()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualStateGroups"/> class.
        /// </summary>
        /// <param name="groups">The visual state groups to add to the groups</param>
        public VisualStateGroups(IEnumerable<VisualStateGroup> groups)
            : base(groups)
        {
        }

        /// <summary>
        /// Gets the State manager that will be used to manage currrent states for the visual state groups
        /// </summary>
        public IStateManager StateManager { get; } = new StateManager();

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualStateGroups"/> class from a single visual state group
        /// </summary>
        /// <param name="group">The visual state group to add to the groups</param>
        public static implicit operator VisualStateGroups(VisualStateGroup group)
        {
            return new VisualStateGroups { group };
        }
    }
}
