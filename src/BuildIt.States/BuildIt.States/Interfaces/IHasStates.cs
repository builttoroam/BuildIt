namespace BuildIt.States.Interfaces
{
    /// <summary>
    /// Entity that has a state manager
    /// </summary>
    public interface IHasStates
    {
        /// <summary>
        /// Accesses the state manager
        /// </summary>
        IStateManager StateManager { get; }
    }
}