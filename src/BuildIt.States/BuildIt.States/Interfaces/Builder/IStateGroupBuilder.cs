namespace BuildIt.States.Interfaces.Builder
{
    public interface IStateGroupBuilder<TState> : IStateBuilder
        where TState : struct
    {
        IEnumStateGroup<TState> StateGroup { get; }
    }
}