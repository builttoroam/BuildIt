namespace BuildIt.States.Interfaces.Builder
{
    /// <summary>
    /// Builder for state definition with element to have property set
    /// </summary>
    /// <typeparam name="TState">The type (enum) of the state</typeparam>
    /// <typeparam name="TElement">The type of the element</typeparam>
    /// <typeparam name="TPropertyValue">The type of the property to be changed</typeparam>
    public interface IStateDefinitionValueBuilder<TState, TElement, TPropertyValue> : IStateDefinitionBuilder<TState>
        where TState : struct
    {
        /// <summary>
        /// The value to set
        /// </summary>
        StateValue<TElement, TPropertyValue> Value { get; }
    }
}