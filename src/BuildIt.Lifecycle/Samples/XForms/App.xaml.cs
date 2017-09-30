﻿using Xamarin.Forms;

namespace StateByState.XForms
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new StateByState.XForms.MainPage();
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