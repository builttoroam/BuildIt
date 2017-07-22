using BuildIt;
using BuildIt.States;
using BuildIt.States.Interfaces;
using System;
using System.Collections.ObjectModel;

namespace States.Sample.Core
{
    /// <summary>
    /// The loading states
    /// </summary>
    public enum LoadingStates
    {
        /// <summary>
        /// Default state
        /// </summary>
        Base,

        /// <summary>
        /// Loading data
        /// </summary>
        UILoading,

        /// <summary>
        /// Data loaded
        /// </summary>
        UILoaded,

        /// <summary>
        /// Data failed to load
        /// </summary>
        UILoadingFailed
    }

    /// <summary>
    /// States for different screen sizes
    /// </summary>
    public enum SizeStates
    {
        /// <summary>
        /// Default
        /// </summary>
        Base,

        /// <summary>
        /// Narrow screen
        /// </summary>
        Narrow,

        /// <summary>
        /// Normal scree
        /// </summary>
        Normal,

        /// <summary>
        /// Large scree
        /// </summary>
        Large
    }

    public enum ItemStates
    {
        Base,
        IsEnabled,
        IsNotEnabled
    }

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
                .DefineState(ItemStates.IsNotEnabled);

        }

        public string Output1 { get; set; }
        public string Output2 { get; set; }

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
    }

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
                var item = new RandomItem { Output1 = Guid.NewGuid() + "", Output2 = Guid.NewGuid() + "" };
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
