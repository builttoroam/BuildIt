using System;
using System.Threading.Tasks;
using Autofac;
using BuildIt;
using BuildIt.Lifecycle;
using StateByState.Services;
using StateByState.XForms.Views;
using Xamarin.Forms;

namespace StateByState.XForms
{
	public class App : Application
	{
		public App ()
		{
           
            //var cnt=new ContentPage();
            // The root page of your application
		    MainPage = new NavigationPage();

            StartApplication();
            //new Views.MainPage());
            //new ContentPage();
		    //var btn = new Button()
		    //{
		    //    Text = "test",

		    //};
		    //btn.Clicked += (s, e) =>
		    //{
		    //    MainPage.Navigation.PushAsync(new MainPage());
		    //};
      //      cnt.Content = new StackLayout
      //      {
      //          VerticalOptions = LayoutOptions.Center,
      //          Children = {
      //              btn,
      //                  new Label {
      //                      XAlign = TextAlignment.Center,
      //                      Text = "Welcome to Xamarin Forms!"

      //                  },
      //                  new Label {
      //                      XAlign = TextAlignment.Center,
      //                      Text = "Welcome to Xamarin Forms!"
      //                  },
      //                  new Label {
      //                      XAlign = TextAlignment.Center,
      //                      Text = "Welcome to Xamarin Forms!"
      //                  }

      //              }


      //      };
        }

	    private async void StartApplication()
	    {

            // Handle when your app starts
            NavigationHelper.Register<MainRegionView, MainPage>(MainRegionView.Main);
            NavigationHelper.Register<MainRegionView, SecondPage>(MainRegionView.Second);
            NavigationHelper.Register<MainRegionView, ThirdPage>(MainRegionView.Third);

            // Associate secondary region states with corresponding native pages
            NavigationHelper.Register<SecondaryRegionView, SeparatePage>(SecondaryRegionView.Main);

            var core = new SampleApplication();
            var wm = new WindowManager(MainPage as NavigationPage, core);
            await core.Startup(builder =>
            {
                builder.RegisterType<BasicDebugLogger>().As<ILogService>();
                builder.RegisterType<Special>().As<ISpecial>();
            });

//            (MainPage as NavigationPage).Navigation.PushAsync(new MainPage());
        }

        protected override void OnStart ()
		{

        }

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

