using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            LogHelper.LogOutput = entry => Debug.Write(entry);

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
