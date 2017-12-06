using Autofac;
using BuildIt.Autofac;
using BuildIt.Logging;
using BuildIt.Logging.Filters;
using BuildIt.ServiceLocation;
using System.Diagnostics;
using System.Reflection;
using Xamarin.Forms;

namespace BuildIt.Forms.Sample
{
    public partial class App : Application
    {
        public App()
        {
            LogHelper.LogOutput = entry => Debug.Write(entry);
            LogHelper.LogService = new BasicLoggerService
            {
                Filter = new OrLogFilter(
                    //new AssemblyNameLogFilter
                    //{
                    //    AssemblyName = typeof(BuildIt.Forms.DesignTimeControl).GetTypeInfo().Assembly.GetName().Name
                    //},
                    new AssemblyNameLogFilter
                    {
                        AssemblyName = typeof(App).GetTypeInfo().Assembly.GetName().Name
                    }
                )
            };

            InitializeComponent();

            var build = new ContainerBuilder();
            var container = build.Build();

            var csl = new AutofacServiceLocator(container);
            var dcontainer = new AutofacDependencyContainer(container);
            ServiceLocator.SetLocatorProvider(() => csl);

            MainPage = new BuildIt.Forms.Sample.MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}