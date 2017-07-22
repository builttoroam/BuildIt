using System;
using System.Reflection;
using BuildIt.States.Interfaces;

namespace BuildIt.States.Typed.Enum
{
    /// <summary>
    /// Definition of a enum state group
    /// </summary>
    /// <typeparam name="TState">The state type</typeparam>
    public class EnumStateGroupDefinition<TState>
        : TypedStateGroupDefinition<TState, EnumStateDefinition<TState>>
        where TState : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumStateGroupDefinition{TState}"/> class.
        /// </summary>
        public EnumStateGroupDefinition()
        {
            if (!typeof(TState).GetTypeInfo().IsEnum)
            {
                throw new ArgumentException("Type argument should be enum", nameof(TState));
            }
        }

        /// <summary>
        /// Defines all states for the enum type
        /// </summary>
        public void DefineAllStates()
        {
            var vals = System.Enum.GetValues(typeof(TState));
            foreach (var enumVal in vals)
            {
                $"Defining state {enumVal}".Log();
                DefineTypedState((TState)enumVal);
            }
        }
    }
}