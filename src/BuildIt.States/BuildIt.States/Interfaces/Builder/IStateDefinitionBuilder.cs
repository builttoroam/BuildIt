namespace BuildIt.States.Interfaces.Builder
{
    /// <summary>
    /// Builder for typed state definition
    /// </summary>
    /// <typeparam name="TState">The type (enum) of the state</typeparam>
    public interface IStateDefinitionBuilder<TState> : IStateGroupBuilder<TState>
        where TState : struct
    {
        /// <summary>
        /// The typed state
        /// </summary>
        IEnumStateDefinition<TState> State { get; }
    }
}