using Android.App;
using Android.OS;
using Autofac;

namespace StateByState.Android
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon",NoHistory = true)]
    public class StartActivity : BaseActivity
    {
        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var core = new SampleApplication();
            var fn = new AcitivityNavigation<PageStates, PageTransitions>(this, core);
            fn.Register<MainActivity>(PageStates.Main);
            fn.Register<SecondActivity>(PageStates.Second);
            //fn.Register<ThirdActivity>(PageStates.Third);
            await core.Startup(builder =>
            {
                builder.RegisterType<Special>().As<ISpecial>();
            });

        }
    }
}