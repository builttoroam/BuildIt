using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BuildIt.Lifecycle.States;
using BuildIt.Lifecycle.States.ViewModel;

namespace BuildIt.Lifecycle
{
   

    public class FrameNavigation<TState,TTransition>:IHasCurrentViewModel 
        where TState : struct
        where TTransition:struct
    {
        public INotifyPropertyChanged CurrentViewModel => StateManager?.CurrentViewModel;


        public IViewModelStateManager<TState, TTransition> StateManager { get; }

        private Frame RootFrame { get; }

        public FrameNavigation(Frame rootFrame,
            IHasViewModelStateManager<TState, TTransition> hasStateManager,
            string registerAs = null)
        {
            var stateManager = hasStateManager.StateManager;
            if (string.IsNullOrWhiteSpace( registerAs ))
            {
                registerAs = hasStateManager.GetType().Name;
            }
            Application.Current.Resources[registerAs] = this;
            RootFrame = rootFrame;
            RootFrame.Tag = registerAs;
            StateManager = stateManager;
            StateManager.StateChanged += StateManager_StateChanged;
        }

        private void StateManager_StateChanged(object sender, StateEventArgs<TState> e)
        {
            var tp = NavigationHelper.TypeForState(e.State);
            if (RootFrame.BackStack.FirstOrDefault()?.SourcePageType == tp)
            {
                RootFrame.GoBack();
            }
            else
            {
                RootFrame.Navigate(tp);
            }
        }


    }
}