using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BuildIt.Lifecycle
{
    /// <summary>
    /// Helper for registering views
    /// </summary>
    public static class LifecycleHelper
    {
        /// <summary>
        /// Registers a type of view
        /// </summary>
        /// <typeparam name="TView">The type of view to register</typeparam>
        /// <returns>A reference to the registered view</returns>
        public static NavigationRegistrationHelper RegisterView<TView>()
            where TView : Page
        {
            return new NavigationRegistrationHelper { ViewType = typeof(TView) };
        }

        /// <summary>
        /// Registers a type of view with an application
        /// </summary>
        /// <typeparam name="TView">The type of view to register</typeparam>
        /// <param name="app">The application</param>
        /// <returns>A reference to the registered view</returns>
        public static NavigationRegistrationHelper RegisterView<TView>(this Application app)
            where TView : Page
        {
            return new NavigationRegistrationHelper { ViewType = typeof(TView) };
        }
    }
}