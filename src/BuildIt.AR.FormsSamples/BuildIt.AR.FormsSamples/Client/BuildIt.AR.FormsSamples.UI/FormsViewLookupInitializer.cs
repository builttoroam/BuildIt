using System;
using BuildIt.AR.FormsSamples.Core.ViewModels;
using BuildIt.AR.FormsSamples.UI.Views;
using MvvmCross.Core.Views;
using MvvmCross.Platform;
using System.Reflection;
using MvvmCross.Platform.IoC;
using System.Linq;
using MvvmCross.Core.ViewModels;
using Xamarin.Forms;

namespace BuildIt.AR.FormsSamples.UI
{
    public class FormsViewLookupInitializer:IViewLookupInitialiser
    {
        private const string ViewModelSuffix = "ViewModel";
        private const string ViewSuffix = "View";
        private const string PageSuffix = "Page";

        public void InitializeViewLookup()
        {
            var viewModelTypes = (from vmType in typeof(HomeViewModel).GetTypeInfo().Assembly.CreatableTypes()
                    let pos = vmType.Name.LastIndexOf(ViewModelSuffix, StringComparison.OrdinalIgnoreCase)
                    where pos > 0 && vmType.GetTypeInfo().IsSubclassOf(typeof(MvxViewModel))
                                  select new {Type = vmType, Key = vmType.Name.Substring(0, pos)})
                .ToDictionary(x => x.Key, x => x.Type);

            var viewTypes = (from vmType in typeof(HomePage).GetTypeInfo().Assembly.CreatableTypes()
                let viewpos = vmType.Name.LastIndexOf(ViewSuffix, StringComparison.OrdinalIgnoreCase)
                let pos = viewpos > 0 ? viewpos : vmType.Name.LastIndexOf(PageSuffix, StringComparison.OrdinalIgnoreCase)
                    where pos > 0 && vmType.GetTypeInfo().IsSubclassOf(typeof(ContentPage))
                    select new { Type = vmType, Key = vmType.Name.Substring(0, pos) })
                .ToDictionary(x => x.Key, x => x.Type);

            var vmLookup = (from vm in viewModelTypes
                from v in viewTypes
                where vm.Key == v.Key
                select new {ViewModel = vm.Value, View = v.Value}).ToDictionary(x => x.ViewModel, x => x.View);
            
           
            var container = Mvx.Resolve<IMvxViewsContainer>();

            container.AddAll(vmLookup);
        }
    }
}
