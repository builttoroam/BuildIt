using System;
using System.Collections.Generic;
using BuildIt.MvvmCross.Interfaces;
using BuildIt.States;
using BuildIt.States.Interfaces;
using MvvmCross.ViewModels;

namespace BuildIt.MvvmCross
{
    public class BaseStateAndTransitions : MvxNotifyPropertyChanged, IStateAndStoryboards
    {
        /// <summary>
        /// Event triggering a storyboard to run
        /// </summary>
        public event EventHandler<DualParameterEventArgs<string, Action>> RunStoryboard;

        /// <summary>
        /// Event triggering a storyboard to stop
        /// </summary>
        public event EventHandler<ParameterEventArgs<string>> StopStoryboard;

        /// <inheritdoc/>
        public IStateManager StateManager { get; } = new StateManager();

        /// <inheritdoc/>
        public T CurrentState<T>()
            where T : struct
        {
            return StateManager.CurrentState<T>();
        }

        /// <inheritdoc/>
        public void ChangePageState<T>(T stateName, bool useTransitions = true)
            where T : struct
        {
            StateManager.GoToState(stateName, useTransitions);
        }

        /// <summary>
        /// Begins a storyboard
        /// </summary>
        protected void BeginStoryboard(string storyboard, Action storyboardCompleted = null)
        {
            RunStoryboard.SafeRaise(this, new object[] { storyboard, storyboardCompleted });
        }

        /// <summary>
        /// Ends a storyboard
        /// </summary>
        /// <param name="storyboard"></param>
        protected void EndStoryboard(string storyboard)
        {
            StopStoryboard.SafeRaise(this, storyboard);
        }
    }
}