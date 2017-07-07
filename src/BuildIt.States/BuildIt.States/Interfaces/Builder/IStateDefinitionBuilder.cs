namespace BuildIt.States.Interfaces.Builder
{
    public interface IStateDefinitionBuilder<TState> : IStateGroupBuilder<TState>
        where TState : struct
    {
        IEnumStateDefinition<TState> State { get; }
    }
}