namespace BuildIt.States.Interfaces
{
    public interface IEnumStateDefinition<TStateEnum> : IStateDefinition 
        where TStateEnum : struct
    {
        TStateEnum EnumState { get; }

    }
}