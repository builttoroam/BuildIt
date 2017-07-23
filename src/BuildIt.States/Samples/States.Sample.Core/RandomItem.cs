using BuildIt;
using BuildIt.States;
using BuildIt.States.Interfaces;

namespace States.Sample.Core
{
    public class RandomItem : NotifyBase
    {
        public IStateManager StateManager { get; } = new StateManager();

        public RandomItem()
        {
            StateManager
                .Group<ItemStates>("cacheRandomItemGroup")
                .DefineState(ItemStates.IsEnabled)
                    .Target(this)
                    .Change(x => x.IsEnabled)
                    .ToValue(true)
                .DefineState(ItemStates.IsNotEnabled)
                .DefineState(ItemStates.IsEnabledTwo)
                    .Target(this)
                    .Change(x => x.IsTwoEnabled)
                    .ToValue(true);
        }

        public string Output1 { get; set; }
        public string Output2 { get; set; }

        public string Output3 { get; set; }

        private bool isEnabled;
        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool isTwoEnabled;
        public bool IsTwoEnabled
        {
            get => isTwoEnabled;
            set
            {
                isTwoEnabled = value;
                OnPropertyChanged();
            }
        }
    }
}