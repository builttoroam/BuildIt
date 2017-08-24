using System.Linq;
using BuildIt.States.Interfaces;
using Xamarin.Forms;

namespace BuildIt.Forms.Controls
{
    public class DesignInfo : NotifyBase
    {
        private IStateGroup[] groups;
        private IStateGroup selectedGroup;
        private IStateDefinition[] states;

        public DesignInfo(Element element)
        {
            Element = element;
            var sm = VisualStateManager.GetVisualStateGroups(element);
            StateManager = sm.StateManager;
        }

        public Element Element { get; set; }

        public IStateGroup[] Groups => groups ?? (groups = StateManager.StateGroups.Select(x => x.Value).ToArray());

        public IStateDefinition[] States
        {
            get
            {
                return states;
            }
            set
            {
                states = value;
                OnPropertyChanged();
            }
        }

        public IStateGroup SelectedGroup
        {
            get
            {
                return selectedGroup;
            }
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