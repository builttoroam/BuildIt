using System;
using BuildIt.States.Interfaces;

namespace BuildIt.States.Loading
{
    /// <summary>
    /// State trigger based on the loading state
    /// </summary>
    /// <typeparam name="TLoadingState"></typeparam>
    public class LoadingTrigger<TLoadingState> : StateTriggerBase
        where TLoadingState : struct
    {
        /// <summary>
        /// Initializes a new loading trigger
        /// </summary>
        /// <param name="manager">The loading manager to track loading state</param>
        public LoadingTrigger(LoadingManager<TLoadingState> manager)
        {
            Manager = manager;
            Manager.LoadingChanged += Manager_LoadingChanged;
        }

        /// <summary>
        /// The loading enum value
        /// </summary>
        public TLoadingState ActiveValue { get; set; }

        /// <summary>
        /// The loading manager for tracking loading state
        /// </summary>
        private LoadingManager<TLoadingState> Manager { get; }


        private void Manager_LoadingChanged(object sender, EventArgs e)
        {
            UpdateIsActive(ActiveValue.Equals(Manager.IsLoadingState));
        }
    }
}