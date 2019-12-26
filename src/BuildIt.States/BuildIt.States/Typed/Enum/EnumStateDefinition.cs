namespace BuildIt.States.Typed.Enum
{
    /// <summary>
    /// Defines a state which is specified by an enum type.
    /// </summary>
    /// <typeparam name="TState">The type (enum) of the state.</typeparam>
    public class EnumStateDefinition<TState> : TypedStateDefinition<TState>
        where TState : struct
    {
        /// <summary>
        /// Gets or sets gets the name of the state.
        /// </summary>
        public override string StateName
        {
            get => State + string.Empty;
            set => value.EnumParse<TState>();
        }
    }
}