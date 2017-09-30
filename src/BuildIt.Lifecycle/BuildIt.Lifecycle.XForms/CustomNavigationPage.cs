using BuildIt.Lifecycle.Interfaces;
using BuildIt.States.Interfaces;
using Xamarin.Forms;

namespace BuildIt.Lifecycle
{
    public class CustomNavigationPage : NavigationPage
    {
        public IApplicationRegion ActiveRegion { get; set; }

        protected override bool OnBackButtonPressed()
        {
            var reg = ActiveRegion as IHasStates;
            if (reg?.StateManager.PreviousStateExists ?? false)
            {
                reg.StateManager.GoBackToPreviousState();
                return true;
            }

            return base.OnBackButtonPressed();
        }
    }
}