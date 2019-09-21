using System.Globalization;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;

namespace BuildIt.UI
{
    public partial class MappingBasePage : Page
    {
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var vm = this.DataContext;
            var mappings = (Application.Current as IApplicationWithMapping).Maps;
            if (vm != null && mappings.TryGetValue(vm.GetType(), out var maps))
            {
                foreach (var map in maps)
                {
                    map.Wire(vm);
                }
            }

            (vm as INavigationViewModel)?.OnAppearing(e.Parameter);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            var vm = this.DataContext;
            var mappings = (Application.Current as IApplicationWithMapping).Maps;
            if (vm != null && mappings.TryGetValue(vm.GetType(), out var maps))
            {
                foreach (var map in maps)
                {
                    map.Unwire(vm);
                }
            }

            (vm as INavigationViewModel)?.OnLeaving();
        }
    }
}