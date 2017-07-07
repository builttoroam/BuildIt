namespace BuildIt.States.Interfaces.Builder
{
    /// <summary>
    /// Builder for state definition with a target elemtn
    /// </summary>
    /// <typeparam name="TState">The type (enum) of the state</typeparam>
    /// <typeparam name="TElement">The type of element</typeparam>
    public interface IStateDefinitionValueTargetBuilder<TState, TElement> : IStateDefinitionBuilder<TState>
        where TState : struct
    {
        /// <summary>
        /// The target element
        /// </summary>
        TElement Target { get; }
    }
}