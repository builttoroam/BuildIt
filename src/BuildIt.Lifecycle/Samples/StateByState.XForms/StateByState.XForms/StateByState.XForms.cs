using BuildIt.Lifecycle;
using StateByState.Services;
using StateByState.XForms.Views;
using Xamarin.Forms;

namespace StateByState.XForms
{

    public class App : Application
    {
        public App()
        {

            //var cnt=new ContentPage();
            // The root page of your application
            MainPage = new CustomNavigationPage();//new ContentPage());

            StartApplication();
            //new Views.MainPage());
            //      var cnt = new ContentPage();
            //      var btn = new Button()
            //      {
            //          Text = "test",

            //      };
            //      btn.Clicked += (s, e) =>
            //      {
            //          MainPage.Navigation.PushAsync(new MainPage());
            //      };
            //      cnt.Content = new StackLayout
            //      {
            //          VerticalOptions = LayoutOptions.Center,
            //          Children = {
            //                    btn,
            //                        new Label {
            //                            XAlign = TextAlignment.Center,
            //                            Text = "Welcome to Xamarin Forms!"

            //                        },
            //                        new Label {
            //                            XAlign = TextAlignment.Center,
            //                            Text = "Welcome to Xamarin Forms!"
            //                        },
            //                        new Label {
            //                            XAlign = TextAlignment.Center,
            //                            Text = "Welcome to Xamarin Forms!"
            //                        }

            //                    }


            //      };
            //(MainPage as NavigationPage).Navigation.PushAsync(cnt);
        }

        private async void StartApplication()
        {

            // Handle when your app starts
            LifecycleHelper.RegisterView<MainPage>().ForState(MainRegionView.Main);
            LifecycleHelper.RegisterView<SecondPage>().ForState(MainRegionView.Second);
            LifecycleHelper.RegisterView<ThirdPage>().ForState(MainRegionView.Third);

            LifecycleHelper.RegisterView<SeparatePage>().ForState(SecondaryRegionView.Main);


            var core = new SampleApplication();
            var wm = new WindowManager(MainPage as CustomNavigationPage, core);
            await core.Startup(
                builder =>
            {
                builder.Register<XFormsSpecial, ISpecial>();
            });

            //            (MainPage as NavigationPage).Navigation.PushAsync(new MainPage());
        }

        protected override void OnStart()
        {

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

