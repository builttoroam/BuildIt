using BuildIt.States.Interfaces;

namespace BuildIt.States
{
    /// <summary>
    /// Definition of a state that's based on a enumeration value
    /// </summary>
    /// <typeparam name="TState">The enum type that defines the state group</typeparam>
    public class TypedStateDefinition<TState> : BaseStateDefinition, ITypedStateDefinition<TState>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypedStateDefinition{TState}"/> class.
        /// Creates an instance based on an enumeration value
        /// </summary>
        /// <param name="state">The enum value for the state definition to be created</param>
        public TypedStateDefinition(TState state)
        {
            State = state;
        }

        /// <summary>
        /// Gets the name of the state
        /// </summary>
        public override string StateName => State + string.Empty;

        /// <summary>
        /// Gets or sets the enum value for the state
        /// </summary>
        public TState State { get; set; }
    }
}