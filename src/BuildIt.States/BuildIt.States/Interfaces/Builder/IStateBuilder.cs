namespace BuildIt.States.Interfaces.Builder
{
    /// <summary>
    /// Entity that has a state manager
    /// </summary>
    public interface IStateBuilder
    {
        /// <summary>
        /// Returns the current statemanager
        /// </summary>
        IStateManager StateManager { get; }
    }
}