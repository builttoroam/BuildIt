namespace BuildIt.States.Interfaces.Builder
{
    public interface IStateDefinitionValueTargetBuilder<TState, TElement> : IStateDefinitionBuilder<TState>
        where TState : struct
    {
        TElement Target { get; }
    }
}