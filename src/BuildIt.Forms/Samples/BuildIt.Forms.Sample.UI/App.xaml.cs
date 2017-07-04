using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Core;
using BuildIt.Autofac;
using BuildIt.ServiceLocation;
using Xamarin.Forms;

namespace BuildIt.Forms.Sample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var build = new ContainerBuilder();
            var container = build.Build();

            var csl = new AutofacServiceLocator(container);
            var dcontainer = new AutofacDependencyContainer(container);
            using (dcontainer.StartUpdate())
            {
                dcontainer.Register<CustomBasicDebugLogger, ILogService>();
            }
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

        private class CustomBasicDebugLogger : BasicDebugLogger
        {
            public override void Debug(string message)
            {
                base.Debug(message);
                System.Diagnostics.Debug.WriteLine(message);
            }

            public override void Exception(string message, Exception ex)
            {
                base.Exception(message, ex);
                System.Diagnostics.Debug.WriteLine(message + " " + ex.StackTrace);

            }
        }
    }
}
