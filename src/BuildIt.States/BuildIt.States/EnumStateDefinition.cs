using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    /// <summary>
    /// Definition of a state that's based on a enumeration value
    /// </summary>
    /// <typeparam name="TState">The enum type that defines the state group</typeparam>
    public class EnumStateDefinition<TState> : StateDefinition, IEnumStateDefinition<TState>
        where TState : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumStateDefinition{TState}"/> class.
        /// Creates an instance based on an enumeration value
        /// </summary>
        /// <param name="state">The enum value for the state definition to be created</param>
        public EnumStateDefinition(TState state)
        {
            EnumState = state;
        }

        /// <summary>
        /// Gets the name of the state
        /// </summary>
        public override string StateName => EnumState + string.Empty;

        /// <summary>
        /// Gets or sets the enum value for the state
        /// </summary>
        public TState EnumState { get; set; }
    }
}