using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using BuiltToRoam;
using BuiltToRoam.Lifecycle.States;
using Java.Util;

namespace StateByState.Android
{
    [Activity(Label = "@string/FirstTitle", Icon = "@drawable/icon")]
    public class MainActivity : BaseActivity
    {
        int count = 1;

        public MainViewModel CurrentViewModel => DataContext as MainViewModel;

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

          


            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            var  button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate
            {
                CurrentViewModel.Test();
                ////button.Text = string.Format("{0} clicks!", count++);
                //var intent = new Intent(this, typeof (SecondActivity));
                //StartActivity(intent);
            };
        }
    }


    public class VisualStateWrapper<TState, TTransition>
        where TState : struct
        where TTransition: struct
    {
        private IStateManager<TState, TTransition> stateManager;
        public IDictionary<TState, Action<bool>> States { get; } =new Dictionary<TState,Action<bool>>();

        public static string StateManagerName => nameof(StateManager);


        public IStateManager<TState, TTransition> StateManager
        {
            get { return stateManager; }
            set
            {
                if (stateManager == value) return;
                if (stateManager != null)
                {
                    stateManager.StateChanged -= StateManager_StateChanged;
                }
                stateManager = value;
                if (stateManager != null)
                {
                    stateManager.StateChanged += StateManager_StateChanged;
                }
            }
        }

        private void StateManager_StateChanged(object sender, StateEventArgs<TState> e)
        {
            var state = States.SafeDictionaryValue<TState, Action<Boolean>, Action<bool>>(e.State);
            state?.Invoke(e.UseTransitions);
        }
    }
}

