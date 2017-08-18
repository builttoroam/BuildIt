using System;

namespace BuildIt.Lifecycle
{
    public class NavigationRegistrationHelper
    {
        public Type ViewType { get; set; }

    }

    public class NavigationStateRegistrationHelper<TState>: NavigationRegistrationHelper
        where TState:struct
    {
        public TState State { get; set; }
    }
}