using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using BuiltToRoam.Lifecycle.States;

namespace StateByState.Android
{
    public static class ActivityStateManager
    {
        public static IDictionary<string, object> Managers { get; } = new Dictionary<string, object>();
    }
    public class AcitivityNavigation<TState, TTransition>
        where TState : struct
        where TTransition : struct
    {
        public IDictionary<TState, Type> NavigationIndex { get; } = new Dictionary<TState, Type>();

        public IStateManager<TState, TTransition> StateManager { get; }

        private BaseActivity RootActivity { get; }

        public AcitivityNavigation(BaseActivity rootActivity,
            IHasStateManager<TState, TTransition> hasStateManager,
            string registerAs = null)
        {
            var stateManager = hasStateManager.StateManager;
            if (registerAs == null)
            {
                registerAs = hasStateManager.GetType().Name;
            }
            //Application.Current.Resources[registerAs] = this;
            ActivityStateManager.Managers[registerAs] = this;
            RootActivity = rootActivity;
            rootActivity.Tag = registerAs;
            StateManager = stateManager;
            StateManager.StateChanged += StateManager_StateChanged;
        }

        private void StateManager_StateChanged(object sender, StateEventArgs<TState> e)
        {
            var tp = NavigationIndex[e.State];
            var intent = new Intent(RootActivity, tp);
            intent.PutExtra("Tag", RootActivity.Tag);
            RootActivity.StartActivity(intent);
            //if (RootActivity.BackStack.FirstOrDefault()?.SourcePageType == tp)
            //{
            //    RootActivity.GoBack();
            //}
            //else
            //{
            //    RootActivity.Navigate(tp);
            //}
        }

        public void Register<TPage>(TState state)
        {
            NavigationIndex[state] = typeof(TPage);
        }

    }
}