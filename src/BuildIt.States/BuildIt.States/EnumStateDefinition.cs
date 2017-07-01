using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    public class EnumStateDefinition<TState> : StateDefinition, IEnumStateDefinition<TState>
        where TState : struct
    {
        public override string StateName => EnumState + "";
        public TState EnumState { get; set; }

        public EnumStateDefinition(TState state)
        {
            EnumState = state;
        }
    }
}