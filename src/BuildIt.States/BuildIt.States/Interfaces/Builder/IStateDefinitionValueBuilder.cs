namespace BuildIt.States.Interfaces.Builder
{
    public interface IStateDefinitionValueBuilder<TState, TElement, TPropertyValue> : IStateDefinitionBuilder<TState>
        where TState : struct
    {
        StateValue<TElement, TPropertyValue> Value { get; }
    }
}