using System;
using System.ComponentModel;
using System.Reflection;

namespace BuildIt.States
{
    /// <summary>
    /// Definition of a enum state group
    /// </summary>
    /// /// <typeparam name="TState">The enum type</typeparam>
    public class EnumStateGroupDefinition<TState> : TypedStateGroupDefinition<TState>
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
        public override void DefineAllStates()
        {
            var vals = Enum.GetValues(typeof(TState));
            foreach (var enumVal in vals)
            {
                $"Defining state {enumVal}".Log();
                DefineTypedState((TState)enumVal);
            }
        }

        /// <summary>
        /// Returns a state from the state name
        /// </summary>
        /// <param name="stateName">The state name to parse</param>
        /// <returns>The state</returns>
        public override TState FromString(string stateName) => Utilities.EnumParse<TState>(stateName);
    }
}