namespace BuildIt.States.Interfaces.Builder
{
    public interface IStateCompletionBuilder<TState, TCompletion> : IStateDefinitionBuilder<TState>
        where TState : struct
        where TCompletion : struct
    {
        TCompletion Completion { get; }
    }
}