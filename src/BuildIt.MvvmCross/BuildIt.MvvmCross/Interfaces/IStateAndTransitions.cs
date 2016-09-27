using System;

namespace BuildIt.MvvmCross.Interfaces
{
    public interface IStateAndTransitions
    {
        /// <summary>
        /// Event to indicate that the page should change state.
        /// Parameter 1: Name of the state to transition to
        /// Parameter 2: Whether to use transitions or not
        /// </summary>
        event EventHandler<DualParameterEventArgs<string, bool>> StateChanged;

        /// <summary>
        /// Method to invoke state change
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stateName"></param>
        /// <param name="useTransitions"></param>
        void ChangePageState<T>(T stateName, bool useTransitions = true) where T : struct;

        /// <summary>
        /// Method to retrieve current state
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T CurrentState<T>() where T : struct;

        /// <summary>
        /// Method to trigger the current state
        /// </summary>
        /// <param name="useTransitions"></param>
        void RefreshStates(bool useTransitions = false);

        /// <summary>
        /// Event to indicate that a storyboard should be run
        /// Parameter 1: Name of the storyboard
        /// Parameter 2: Action to invoke when storyboard completes (can be null)
        /// </summary>
        event EventHandler<DualParameterEventArgs<string, Action>> RunStoryboard;

        /// <summary>
        /// Event to indicate that a storyboard should be stopped
        /// Parameter 1: Name of the storyboard
        /// </summary>
        event EventHandler<ParameterEventArgs<string>> StopStoryboard;
    
    }
}
