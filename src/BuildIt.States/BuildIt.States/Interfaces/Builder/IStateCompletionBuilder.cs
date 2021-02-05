namespace BuildIt.States.Interfaces.Builder
{
    /// <summary>
    /// Builder for a state with a completion.
    /// </summary>
    /// <typeparam name="TState">The type (enum) of the state.</typeparam>
    /// <typeparam name="TCompletion">The type (enum) of the completion.</typeparam>
    public interface IStateCompletionBuilder<TState, TCompletion> : IStateDefinitionBuilder<TState>
        where TState : struct
        where TCompletion : struct
    {
        /// <summary>
        /// Gets the completion value.
        /// </summary>
        TCompletion Completion { get; }
    }
}