namespace BuildIt.Lifecycle
{
    public class NavigationStateRegistrationHelper<TState> : NavigationRegistrationHelper
        where TState : struct
    {
        public TState State { get; set; }
    }
}