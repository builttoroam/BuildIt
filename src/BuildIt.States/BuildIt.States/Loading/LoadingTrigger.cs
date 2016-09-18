using System;

namespace BuildIt.States.Loading
{
    public class LoadingTrigger<TLoadingState> : StateTriggerBase
        where TLoadingState : struct
    {
        public TLoadingState ActiveValue { get; set; }

        LoadingManager<TLoadingState> Manager { get; set; }

        public LoadingTrigger(LoadingManager<TLoadingState> manager)
        {
            Manager = manager;
            Manager.LoadingChanged += Manager_LoadingChanged;
        }

        private void Manager_LoadingChanged(object sender, EventArgs e)
        {
            UpdateIsActive(ActiveValue.Equals(Manager.IsLoadingState));
        }
    }
}