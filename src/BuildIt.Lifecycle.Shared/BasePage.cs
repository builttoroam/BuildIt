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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var mgr = Application.Current.Resources[Frame.Tag];
            if (mgr == null) return;

            var props = mgr.GetType().GetTypeInfo().DeclaredProperties.FirstOrDefault(p => p.Name == "StateManager");
            var manager = props?.GetValue(mgr) as IHasCurrentViewModel;
            var dc = manager?.CurrentViewModel;
            DataContext = dc;


            var pps = dc.GetType().GetTypeInfo().DeclaredProperties;
            foreach (var p in pps)
            {

                var pt = p.PropertyType.GetTypeInfo();
                var interfaces = pt.ImplementedInterfaces;
                if (pt.IsInterface)
                {
                    interfaces = new[] { pt.AsType() }.Union(interfaces);
                }
                var ism = typeof(IStateManager<,>);
                var vsct = typeof(VisualStateChanger<,>);
                foreach (var inf in interfaces)
                {
                    if (inf.IsConstructedGenericType &&
                        inf.GetGenericTypeDefinition() == ism)
                    {
                        var parm = inf.GenericTypeArguments;
                        var vsc = Activator.CreateInstance(vsct.MakeGenericType(parm), this, p.GetValue(dc));
                    }
                }
            }
        }
    }
}