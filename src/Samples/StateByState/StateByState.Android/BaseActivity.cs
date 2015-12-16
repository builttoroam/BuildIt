using System.Linq;
using System.Reflection;
using Android.App;
using Android.OS;
using BuiltToRoam;
using BuiltToRoam.Lifecycle.States;
using BuiltToRoam.Lifecycle.States.ViewModel;

namespace StateByState.Android
{
    public class BaseActivity : Activity
    {
        public string Tag { get; set; }

        public object DataContext { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Tag=this.Intent.GetStringExtra("Tag");

            if (string.IsNullOrWhiteSpace(Tag)) return;

            var mgr = ActivityStateManager.Managers.SafeDictionaryValue<string,object,object>(Tag);
            if (mgr == null) return;

            var props = mgr.GetType().GetTypeInfo().DeclaredProperties.FirstOrDefault(p => p.Name == "StateManager");
            var manager = props?.GetValue(mgr) as IHasCurrentViewModel;
            var dc = manager?.CurrentViewModel;
            DataContext = dc;
            
            var actpps = this.GetType().GetTypeInfo().DeclaredProperties;


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
                foreach (var inf in interfaces)
                {
                    if (inf.IsConstructedGenericType &&
                        inf.GetGenericTypeDefinition() == ism)
                    {
                        var parm = inf.GenericTypeArguments;

                        var vsct = typeof (VisualStateWrapper<,>).MakeGenericType(parm);
                        foreach (var px in actpps)
                        {
                            if (px.PropertyType == vsct)
                            {
                                var vsw = px.GetValue(this);
                                var pmgr = vsw.GetType().GetProperty(VisualStateWrapper<int, int>.StateManagerName);
                                pmgr.SetValue(vsw, p.GetValue(dc));
                            }
                        }
                    }
                }
            }
        }
    }
}