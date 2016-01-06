using System;
using System.Collections.Generic;

namespace BuildIt.Lifecycle
{
    public class NavigationRegistrationHelper
    {
        public Type ViewType { get; set; }
    }

    public static class NavigationHelper
    {
        private static IDictionary<string, Type> NavigationIndex { get; } = new Dictionary<string, Type>();

        public static void Register<TStateInfo>(TStateInfo state, Type viewType)
            where TStateInfo : struct

        {
            var key = KeyFromState(state);
            NavigationIndex[key] = viewType;
        }

        public static void ForState<TState>(this NavigationRegistrationHelper reg, TState state) where TState : struct
        {
            Register(state, reg.ViewType);
        }

        private static string KeyFromState<TStateInfo>(TStateInfo state)
        {
            var type = state.GetType().Name;
            return type + state;
        }


        public static Type TypeForState<TStateInfo>(TStateInfo state)
        {
            return NavigationIndex[KeyFromState(state)];

        }
    }
}