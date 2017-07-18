using System;
using System.Reflection;

namespace BuildIt.States
{
    /// <summary>
    /// A typed state group
    /// </summary>
    /// <typeparam name="TState">The type (enum) fo the state group</typeparam>
    public class EnumStateGroup<TState> : TypedStateGroup<TState>
        where TState : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumStateGroup{TState}"/> class.
        /// </summary>
        public EnumStateGroup()
        {
            if (!typeof(TState).GetTypeInfo().IsEnum)
            {
                throw new ArgumentException("Type argument should be enum", nameof(TState));
            }
        }

        /// <summary>
        /// Gets or sets the current state name
        /// </summary>
        public override string CurrentStateName
        {
            get => (!default(TState).Equals(CurrentState)) ? CurrentState + string.Empty : null;
            protected set => CurrentState = value.EnumParse<TState>();
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
        protected override TState FromString(string stateName) => Utilities.EnumParse<TState>(stateName);
    }
}