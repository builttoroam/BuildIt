using BuildIt;
using BuildIt.States;
using BuildIt.States.Interfaces;

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

    /// <summary>
    /// The view model for the main page
    /// </summary>
    public class MainViewModel : NotifyBase, IHasStates
    {
        private string currentStateName = "Test data";

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
