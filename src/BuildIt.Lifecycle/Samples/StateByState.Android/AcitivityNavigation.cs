using Android.Content;
using BuildIt.States.Interfaces;
using BuildIt.States.Typed;
using System;
using System.Collections.Generic;

namespace StateByState.Android
{
    public static class ActivityStateManager
    {
        public static IDictionary<string, object> Managers { get; } = new Dictionary<string, object>();
    }

    public class AcitivityNavigation<TState>
        where TState : struct
    {
        public IDictionary<TState, Type> NavigationIndex { get; } = new Dictionary<TState, Type>();

        public IStateManager StateManager { get; }

        private BaseActivity RootActivity { get; }

        public AcitivityNavigation(BaseActivity rootActivity,
            IHasStates hasStateManager,
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
            //StateManager.StateChanged += StateManager_StateChanged;
        }

        private void StateManager_StateChanged(object sender, TypedStateEventArgs<TState> e)
        {
            var tp = NavigationIndex[e.TypedState];
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