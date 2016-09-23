using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using BuildIt.Lifecycle.States;
using BuildIt.Lifecycle.States.ViewModel;

namespace BuildIt.Lifecycle
{
    public class BasePage : Page
    {
        public INotifyPropertyChanged ViewModel => DataContext as INotifyPropertyChanged;

        public BasePage()
        {
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    base.OnNavigatedTo(e);

        //    //"Locating the frame navigation".Log();
        //    //var mgr = Application.Current.Resources[Frame.Tag];
        //    //if (mgr == null) return;

        //    //$"Frame navigation located '{mgr.GetType().Name}', now looking for state manager".Log();
        //    ////var allprops = mgr.GetType().GetTypeInfo().DeclaredProperties;
        //    ////foreach (var p in allprops)
        //    ////{
        //    ////    $"Property {p.Name}".Log();
        //    ////}
        //    ////var props = allprops.FirstOrDefault(p => p.Name == "StateManager");
        //    ////var props = mgr.GetType().GetRuntimeProperty("StateManager");
        //    ////$"Found runtime property? '{props!=null}'".Log();
        //    ////props?.GetValue(mgr) 

        //    //var manager = e.Parameter as IHasCurrentViewModel;
        //    //$"Found current view model manager? '{manager!=null}'".Log();
        //    //var dc = manager?.CurrentViewModel;

        //    var dc = e.Parameter as INotifyPropertyChanged;

        //    $"Found data context? '{dc!=null}'".Log();
        //    DataContext = dc;


        //    var pps = dc.GetType().GetTypeInfo().DeclaredProperties;
        //    "Iterating through declared properties".Log();
        //    foreach (var p in pps)
        //    {
        //        var pt = p.PropertyType.GetTypeInfo();
        //        var interfaces = pt.ImplementedInterfaces;
        //        if (pt.IsInterface)
        //        {
        //            interfaces = new[] { pt.AsType() }.Union(interfaces);
        //        }
        //        "Completed interface search".Log();
        //        var ism = typeof(IStateManager<,>);
        //        var vsct = typeof(VisualStateChanger<,>);
        //        foreach (var inf in interfaces)
        //        {
        //            $"Inspecting interface {inf.Name}".Log();
        //            if (inf.IsConstructedGenericType &&
        //                inf.GetGenericTypeDefinition() == ism)
        //            {
        //                "Interface matched, creating instance".Log();
        //                var parm = inf.GenericTypeArguments;
        //                var vsc = Activator.CreateInstance(vsct.MakeGenericType(parm), this, p.GetValue(dc));
        //                "Instance created".Log();

        //                //var uiRequired = p.GetValue(dc) as IRequiresUIAccess;
        //                //if (uiRequired != null)
        //                //{
        //                //    uiRequired.UIContext.RunContext = (mgr as IRequiresUIAccess)?.UIContext.RunContext;
        //                //}
        //            }
        //        }
        //    }
        //}
    }
}