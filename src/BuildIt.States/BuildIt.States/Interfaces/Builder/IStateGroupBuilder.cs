namespace BuildIt.States.Interfaces.Builder
{
    /// <summary>
    /// Builder for state group
    /// </summary>
    /// <typeparam name="TState">The type (enum) of the state group</typeparam>
    public interface IStateGroupBuilder<TState> : IStateBuilder
        where TState : struct
    {
        /// <summary>
        /// Gets the typed state group
        /// </summary>
        ITypedStateGroup<TState> StateGroup { get; }

        /// <summary>
        /// Gets Optional caching tag for state group
        /// </summary>
        string StateGroupTag { get; }

        /// <summary>
        /// Gets a value indicating whether it's using a cached deffinition
        /// </summary>
        bool IsCachedDefinition { get; }

        /// <summary>
        /// The node index for determining which target element to use for specific node
        /// </summary>
        int NodeIndex { get; set; }
    }
}