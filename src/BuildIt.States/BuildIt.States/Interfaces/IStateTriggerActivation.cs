namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Trigger whose active state can be updated
    /// </summary>
    public interface IStateTriggerActivation
    {
        /// <summary>
        /// Updates the active state
        /// </summary>
        /// <param name="isActive">Whether the trigger is active</param>
        void UpdateIsActive(bool isActive);
    }
}