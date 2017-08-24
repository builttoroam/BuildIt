using System.Linq;
using BuildIt.States.Interfaces;
using Xamarin.Forms;

namespace BuildIt.Forms.Controls
{
    /// <summary>
    /// Supporting class for the DesignTimeControl (shouldn't be used in isolation)
    /// </summary>
    public class DesignInfo : NotifyBase
    {
        private IStateGroup[] groups;
        private IStateGroup selectedGroup;
        private IStateDefinition[] states;

        /// <summary>
        /// Initializes a new instance of the <see cref="DesignInfo"/> class.
        /// </summary>
        /// <param name="element">The element which contains the visual states</param>
        public DesignInfo(Element element)
        {
            Element = element;
            var sm = VisualStateManager.GetVisualStateGroups(element);
            StateManager = sm.StateManager;
        }

        /// <summary>
        /// Gets the element with the visual states
        /// </summary>
        public Element Element { get; }

        /// <summary>
        /// Gets the state groups that are available
        /// </summary>
        public IStateGroup[] Groups => groups ?? (groups = StateManager.StateGroups.Select(x => x.Value).ToArray());

        /// <summary>
        /// Gets or sets the states available in the currently selected group
        /// </summary>
        public IStateDefinition[] States
        {
            get => states;
            set
            {
                states = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the selected group
        /// </summary>
        public IStateGroup SelectedGroup
        {
            get => selectedGroup;
            set
            {
                selectedGroup = value;
                if (selectedGroup == null)
                {
                    return;
                }

                States = selectedGroup.GroupDefinition.States.Select(x => x.Value).ToArray();
            }
        }

        private IStateManager StateManager { get; }
    }
}