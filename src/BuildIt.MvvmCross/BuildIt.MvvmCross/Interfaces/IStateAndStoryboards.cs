using BuildIt.States.Interfaces;
using System;

namespace BuildIt.MvvmCross.Interfaces
{
    /// <summary>
    /// Interface that defines states and storyboards.
    /// </summary>
    public interface IStateAndStoryboards : IHasStates
    {
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

        /// <summary>
        /// Method to invoke state change.
        /// </summary>
        /// <typeparam name="T">The state group.</typeparam>
        /// <param name="stateName">The state. </param>
        /// <param name="useTransitions">Whether to use transitions.</param>
        void ChangePageState<T>(T stateName, bool useTransitions = true)
            where T : struct;

        /// <summary>
        /// Method to retrieve current state.
        /// </summary>
        /// <typeparam name="T">The state group.</typeparam>
        /// <returns>The current state.</returns>
        T CurrentState<T>()
            where T : struct;
    }
}