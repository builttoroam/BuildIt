namespace BuildIt.States.Interfaces.Builder
{
    /// <summary>
    /// Entity that has a state manager
    /// </summary>
    public interface IStateBuilder
    {
        /// <summary>
        /// Gets returns the current statemanager
        /// </summary>
        IStateManager StateManager { get; }
    }
}