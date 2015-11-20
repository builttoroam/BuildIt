using System;
using System.Collections.Generic;
using BuildIt.MvvmCross.Interfaces;
using Cirrious.MvvmCross.ViewModels;

namespace BuildIt.MvvmCross
{
    public class BaseStateAndTransitions : MvxNotifyPropertyChanged,IStateAndTransitions
    {
        /// <summary>
        /// Event triggering a storyboard to run
        /// </summary>
        public event EventHandler<DualParameterEventArgs<string, Action>> RunStoryboard;

        /// <summary>
        /// Event triggering a storyboard to stop
        /// </summary>
        public event EventHandler<ParameterEventArgs<string>> StopStoryboard;

        /// <summary>
        /// Event indicating that a control should change state
        /// </summary>
        public event EventHandler<DualParameterEventArgs<string, bool>> StateChanged;

        private readonly Dictionary<string, string> currentStates = new Dictionary<string, string>();
        public T CurrentState<T>() where T : struct
        {
            var current = currentStates.SafeDictionaryValue<string, string, string>(typeof(T).FullName);
            var tvalue = current.EnumParse<T>();
            return tvalue;
        }

        public void ChangePageState<T>(T stateName, bool useTransitions = true) where T : struct
        {
            var current = currentStates.SafeDictionaryValue<string, string, string>(typeof(T).FullName);

            var attrib = ((Enum)(object)stateName).GetAttribute<VisualStateAttribute>();
            string newState;

            if (attrib != null)
            {
                newState = attrib.VisualStateName;
            }
            else
            {
                newState = stateName.ToString();
            }

            if (string.IsNullOrWhiteSpace(current) || current != newState)
            {
                currentStates[typeof(T).FullName] = newState;
                StateChanged.SafeRaise(this, newState, useTransitions);
            }
        }

        public void RefreshStates(bool useTransitions = false)
        {
            foreach (var currentState in currentStates)
            {
                StateChanged.SafeRaise(this, currentState.Value, useTransitions);
            }
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