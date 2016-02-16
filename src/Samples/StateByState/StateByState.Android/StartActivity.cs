using Android.App;
using Android.OS;
using Autofac;
using StateByState.Services;

namespace StateByState.Android
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon",NoHistory = true)]
    public class StartActivity : BaseActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //var core = new SampleApplication();
            //var fn = new AcitivityNavigation<MainRegionView>(this, core);
            //fn.Register<MainActivity>(MainRegionView.Main);
            //fn.Register<SecondActivity>(MainRegionView.Second);
            ////fn.Register<ThirdActivity>(PageStates.Third);
            //await core.Startup(builder =>
            //{
            //    //builder.RegisterType<Special>().As<ISpecial>();
            //});

        }
    }
}