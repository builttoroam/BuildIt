using Xamarin.Forms;

namespace BuildIt.Lifecycle
{
    public static class LifecycleHelper
    {
        public static NavigationRegistrationHelper RegisterView<TView>()
            where TView : Page
        {
            return new NavigationRegistrationHelper { ViewType = typeof(TView) };
        }
    }
}