using BuildIt.MvvmCross.Interfaces;
using BuildIt.States;
using BuildIt.States.Interfaces;
using MvvmCross.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.MvvmCross.ViewModels
{
    public class BaseViewModel : MvxViewModel, IStateAndStoryboards, ICanGoBack
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

        public void RefreshStates(bool useTransitions = false)
        {
            StateManager.RefreshStates(useTransitions);
        }

        /// <summary>
        /// Begins a storyboard.
        /// </summary>
        protected void BeginStoryboard(string storyboard, Action storyboardCompleted = null)
        {
            RunStoryboard.SafeRaise(this, new object[] { storyboard, storyboardCompleted });
        }

        /// <summary>
        /// Ends a storyboard.
        /// </summary>
        /// <param name="storyboard"></param>
        protected void EndStoryboard(string storyboard)
        {
            StopStoryboard.SafeRaise(this, storyboard);
        }

        public event EventHandler ClearPreviousViews;

#pragma warning disable 1998 // Async to allow for implementation

        public async virtual Task GoingBack(CancelEventArgs e)
#pragma warning restore 1998
        {
            // Do nothing - method is here so it can be inherit
        }

        protected void ClearPrevious()
        {
            ClearPreviousViews.SafeRaise(this);
        }

        private ManualResetEvent waiter = new ManualResetEvent(false);

        public async override void Start()
        {
            base.Start();

            try
            {
                await Task.Yield();

                await StartAsync();
            }
            finally
            {
                waiter.Set();
            }
        }

#pragma warning disable 1998 // Returns a Task so that overrides can do async work

        public virtual async Task StartAsync()
#pragma warning restore 1998
        {
        }

        public async Task WaitForStartCompleted()
        {
            await Task.Run(() => waiter.WaitOne());
        }
    }
}