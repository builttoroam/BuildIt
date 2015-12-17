using System;
using System.Collections.Generic;

namespace BuildIt.Lifecycle
{
    public static class NavigationHelper
    {
        private static IDictionary<string, Type> NavigationIndex { get; } = new Dictionary<string, Type>();

        public static void Register<TStateInfo, TPage>(TStateInfo state)
            where TStateInfo : struct

        {
            var key = KeyFromState<TStateInfo>(state);
            NavigationIndex[key] = typeof(TPage);
        }

        private static string KeyFromState<TStateInfo>(TStateInfo state)
        {
            var type = state.GetType().Name;
            return type + state.ToString();
        }


        public static Type TypeForState<TStateInfo>(TStateInfo state)
        {
            return NavigationIndex[KeyFromState(state)];

        }
    }
}