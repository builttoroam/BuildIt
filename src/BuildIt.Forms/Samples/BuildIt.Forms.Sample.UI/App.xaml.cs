using BuildIt.Logging;
using BuildIt.Logging.Filters;
using System.Diagnostics;
using System.Reflection;
using Xamarin.Forms;

namespace BuildIt.Forms.Sample
{
    public partial class App : Application
    {
        public App()
        {
            LogHelper.LogOutput = entry => Debug.WriteLine(entry);
            LogHelper.LogService = new BasicLoggerService
            {
                Filter = new OrLogFilter(
                    new AssemblyNameLogFilter
                    {
                        AssemblyName = typeof(App).GetTypeInfo().Assembly.GetName().Name
                    })
            };

            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }
    }
}