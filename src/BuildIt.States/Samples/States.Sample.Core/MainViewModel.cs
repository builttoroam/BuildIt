using BuildIt;
using BuildIt.States;
using BuildIt.States.Interfaces;
using System;
using System.Collections.ObjectModel;

namespace States.Sample.Core
{
    /// <summary>
    /// The view model for the main page
    /// </summary>
    public class MainViewModel : NotifyBase, IHasStates
    {
        private string currentStateName = "Test data";

        public ObservableCollection<RandomItem> RandomItems { get; } = new ObservableCollection<RandomItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            StateManager.Group<LoadingStates>().DefineAllStates()
                .Group<SizeStates>().DefineAllStates();

            StateManager.Group<SizeStates>()
                .DefineState(SizeStates.Narrow)
                .ChangePropertyValue(() => CurrentStateName, "Narrow")
                .DefineState(SizeStates.Normal)
                .ChangePropertyValue(vm => CurrentStateName, "Normal")
                .DefineState(SizeStates.Large)
                .ChangePropertyValue(vm => CurrentStateName, "Large");

            var rnd = new Random();
            for (int i = 0; i < 1000; i++)
            {
                var item = new RandomItem {
                    Output1 = Guid.NewGuid() + string.Empty,
                    Output2 = Guid.NewGuid() + string.Empty,
                    Output3 = Guid.NewGuid() + string.Empty
                };
                var enabled = (rnd.Next(0, 1000) < 500);
                RandomItems.Add(item);
                item.StateManager.GoToState(enabled ? ItemStates.IsEnabled : ItemStates.IsNotEnabled);
            }
            RandomItems[0].StateManager.GoToState(ItemStates.IsEnabled);
            RandomItems[5].StateManager.GoToState(ItemStates.IsEnabled);
        }

        /// <summary>
        /// Gets state manager instance
        /// </summary>
        public IStateManager StateManager { get; } = new StateManager();

        /// <summary>
        /// Gets or sets the current state name
        /// </summary>
        public string CurrentStateName
        {
            get => currentStateName;
            set
            {
                currentStateName = value;
                OnPropertyChanged();
            }
        }
    }
}
