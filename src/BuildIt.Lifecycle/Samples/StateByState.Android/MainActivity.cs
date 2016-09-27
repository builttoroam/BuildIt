using Android.App;
using Android.OS;
using Android.Widget;
using BuildIt;
using BuildIt.States;
using System;
using System.Collections.Generic;

namespace StateByState.Android
{
    [Activity(Label = "@string/FirstTitle", Icon = "@drawable/icon")]
    public class MainActivity : BaseActivity
    {
        public MainViewModel CurrentViewModel => DataContext as MainViewModel;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);




            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            var button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate
            {
                CurrentViewModel.Test();
                ////button.Text = string.Format("{0} clicks!", count++);
                //var intent = new Intent(this, typeof (SecondActivity));
                //StartActivity(intent);
            };
        }
    }


    public class VisualStateWrapper<TState>
        where TState : struct
    {
        private IStateManager stateManager;
        public IDictionary<TState, Action<bool>> States { get; } = new Dictionary<TState, Action<bool>>();

        public static string StateManagerName => nameof(StateManager);


        public IStateManager StateManager
        {
            get { return stateManager; }
            set
            {
                //if (stateManager == value) return;
                //if (stateManager != null)
                //{
                //    stateManager.StateChanged -= StateManager_StateChanged;
                //}
                //stateManager = value;
                //if (stateManager != null)
                //{
                //    stateManager.StateChanged += StateManager_StateChanged;
                //}
            }
        }

        private void StateManager_StateChanged(object sender, StateEventArgs<TState> e)
        {
            var state = States.SafeValue(e.State);
            state?.Invoke(e.UseTransitions);
        }
    }
}

