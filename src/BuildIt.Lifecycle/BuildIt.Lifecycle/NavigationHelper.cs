using System;
using System.Collections.Generic;

namespace BuildIt.Lifecycle
{
    /// <summary>
    /// Helper methods to aid with navigation
    /// </summary>
    public static class NavigationHelper
    {
        private static IDictionary<string, Type> NavigationIndex { get; } = new Dictionary<string, Type>();

        /// <summary>
        /// Registers a view to a state
        /// </summary>
        /// <typeparam name="TState">The type of state</typeparam>
        /// <param name="reg">The reference to the type of view</param>
        /// <param name="state">The state to register the view against</param>
        public static void ForState<TState>(this NavigationRegistrationHelper reg, TState state)
            where TState : struct
        {
            Register(state, reg.ViewType);
        }

        /// <summary>
        /// Register a type of view against a specific state
        /// </summary>
        /// <typeparam name="TStateInfo">The type of state</typeparam>
        /// <param name="state">The state</param>
        /// <param name="viewType">The type of view to register</param>
        public static void Register<TStateInfo>(TStateInfo state, Type viewType)
                    where TStateInfo : struct
        {
            var key = KeyFromState(state);
            NavigationIndex[key] = viewType;
        }

        /// <summary>
        /// Retrieves the view type that's registered for a specific state
        /// </summary>
        /// <typeparam name="TStateInfo">The type of state</typeparam>
        /// <param name="state">The state</param>
        /// <returns>The type of the registered view, or null if not found</returns>
        public static Type TypeForState<TStateInfo>(TStateInfo state)
        {
            return NavigationIndex.SafeValue(KeyFromState(state));
        }

        private static string KeyFromState<TStateInfo>(TStateInfo state)
        {
            var type = state.GetType().Name;
            return type + state;
        }
    }
}