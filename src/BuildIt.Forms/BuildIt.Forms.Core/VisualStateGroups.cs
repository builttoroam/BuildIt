using BuildIt.States;
using BuildIt.States.Interfaces;
using System.Collections.Generic;

namespace BuildIt.Forms
{
    /// <summary>
    /// Group of visual states in xaml.
    /// </summary>
    public class VisualStateGroups : List<VisualStateGroup>
    {
        /// <summary>
        /// Gets the State manager that will be used to manage currrent states for the visual state groups.
        /// </summary>
        public IStateManager StateManager { get; } = new StateManager();

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualStateGroups"/> class from a single visual state group.
        /// </summary>
        /// <param name="group">The visual state group to add to the groups.</param>
        public static implicit operator VisualStateGroups(VisualStateGroup group)
        {
            return new VisualStateGroups { group };
        }
    }
}