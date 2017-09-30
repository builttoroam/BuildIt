using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.Synchronization
{
    /// <summary>
    /// Context for tracking synchronisation
    /// </summary>
    /// <typeparam name="TSynchronizationStages">The type defining the states</typeparam>
    public class SynchronizationContext<TSynchronizationStages> :
        INotifySynchronizationChanged<TSynchronizationStages>,
        ISynchronizationContext<TSynchronizationStages>
        where TSynchronizationStages : struct
    {
        private const double OneHundred = 100;

        private readonly object cancelLock = new object();

        private readonly List<ISynchronizationStage<TSynchronizationStages>> synchronizationSteps = new List<ISynchronizationStage<TSynchronizationStages>>();

        private readonly AutoResetEvent syncWaiter = new AutoResetEvent(true);

        private CancellationTokenSource cancellationSource;

        /// <summary>
        /// Event stating that synchronization has changed
        /// </summary>
        public event EventHandler<SynchronizationEventArgs<TSynchronizationStages>> SynchronizationChanged;

        public async Task Cancel(bool waitForSynchronizationToComplete = false)
        {
            var syncTask = Task.Run(() => BackgroundThreadCancel());
            if (waitForSynchronizationToComplete)
            {
                await syncTask;
            }
        }

        public void DefineSynchronizationStep(
                    TSynchronizationStages synchronizationStage,
                    Func<ISynchronizationStage<TSynchronizationStages>, Task<bool>> stepAction)
        {
            var wrapper = SyncStepWrapper<TSynchronizationStages>.Build(
                synchronizationStage,
                stepAction);

            synchronizationSteps.Add(wrapper);
        }

        /// <summary>
        /// Raises synchronization state changed
        /// </summary>
        /// <param name="action">The sync action</param>
        /// <param name="stage">The sync stage</param>
        /// <param name="child">The child event</param>
        /// <param name="percentageComplete">The percent complet (approx)</param>
        /// <param name="error">Whether there has been an exception</param>
        public void OnSynchronizationChanged(
            SyncAction action,
            TSynchronizationStages? stage = null,
            ISynchronizationEventArgs child = null,
            double? percentageComplete = null,
            Exception error = null)
        {
            var args = new SynchronizationEventArgs<TSynchronizationStages>
            {
                PercentageComplete = percentageComplete ?? 0,
                Stage = stage,
                Action = action,
                Error = error,
                ChildStage = child
            };

            if (SynchronizationChanged == null)
            {
                return;
            }

            SynchronizationChanged(this, args);
        }

        /// <summary>
        /// Triggers synchronization
        /// </summary>
        /// <param name="stagesToSynchronize">The states to sync</param>
        /// <param name="cancelExistingSynchronization">Whether to cancel any existing syncs that are in progress</param>
        /// <param name="waitForSynchronizationToComplete">Whether to wait for sync to complete</param>
        /// <returns>Task to await</returns>
        public async Task Synchronize(
            TSynchronizationStages stagesToSynchronize,
            bool cancelExistingSynchronization = false,
            bool waitForSynchronizationToComplete = false)
        {
            var syncTask = InternalSync(
                stagesToSynchronize,
                cancelExistingSynchronization);
            if (waitForSynchronizationToComplete)
            {
                await syncTask;
            }
        }

        private void BackgroundThreadCancel()
        {
            try
            {
                lock (cancelLock)
                {
                    var currentCancellation = cancellationSource;
                    if (currentCancellation != null)
                    {
                        currentCancellation.Cancel();
                    }

                    syncWaiter.WaitOne(); // This will wait for sync to finish
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                syncWaiter.Set();     // This will make sure no one is blocked on the cancel operation
            }
        }

        private async Task BackgroundThreadSync(TSynchronizationStages stagesToSynchronize)
        {
            var cancel = new CancellationTokenSource();
            try
            {
                syncWaiter.WaitOne();
                lock (cancelLock)
                {
                    cancellationSource = cancel;
                }

                var steps = (from s in synchronizationSteps
                             where (((int)(object)s.Stage) & ((int)(object)stagesToSynchronize)) > 0
                             select s).ToArray();

                if (steps == null || steps.Length == 0)
                {
                    OnSynchronizationChanged(SyncAction.Start, percentageComplete: 0.0);
                    OnSynchronizationChanged(SyncAction.End, percentageComplete: OneHundred);
                    return;
                }

                var percentage = 0.0;
                var increment = (1.0 / (double)steps.Length) * 100.0;
                OnSynchronizationChanged(SyncAction.Start, percentageComplete: 0.0);
                foreach (var step in steps)
                {
                    try
                    {
                        if (cancel.Token.IsCancellationRequested)
                        {
                            return;
                        }

                        var progEvent = SynchronizationEventArgs<TSynchronizationStages>.Build(
                            SyncAction.Start,
                            step.Stage);

                        OnSynchronizationChanged(SyncAction.Progress, null, progEvent.Progress(SyncAction.Start, 0.0), percentage);
                        OnSynchronizationChanged(SyncAction.Progress, null, progEvent.Progress(SyncAction.Progress), percentage);
                        Action<ISynchronizationEventArgs> callback = percent =>
                        {
                            var inc = increment * percent.PercentageComplete;
                            OnSynchronizationChanged(SyncAction.Progress, null, percent, percentage + inc);
                        };

                        step.CancellationToken = cancel.Token;
                        step.SynchronizationChanged = callback;
                        try
                        {
                            var ok = await step.StepAction(step); // InvokeStepAction(cancel.Token, callback);
                            if (cancel.Token.IsCancellationRequested)
                            {
                                return;
                            }

                            if (!ok)
                            {
                                OnSynchronizationChanged(SyncAction.Error, null, progEvent.Progress(SyncAction.Error));
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            OnSynchronizationChanged(SyncAction.Error, null, progEvent.Progress(SyncAction.Error, error: ex));
                            throw;
                        }
                        finally
                        {
                            // Make sure we clean up references to the token and callback
                            step.CancellationToken = default(CancellationToken);
                            step.SynchronizationChanged = null;
                        }

                        percentage += increment;
                        OnSynchronizationChanged(SyncAction.Progress, null, progEvent.Progress(SyncAction.Progress, OneHundred), percentage);
                        OnSynchronizationChanged(SyncAction.Progress, null, progEvent.Progress(SyncAction.End), percentage);
                    }
                    catch (Exception ex)
                    {
                        OnSynchronizationChanged(SyncAction.Error, step.Stage, error: ex);
                        throw;
                    }
                }

                OnSynchronizationChanged(SyncAction.End, percentageComplete: OneHundred);
            }
            catch (Exception ex)
            {
                OnSynchronizationChanged(SyncAction.Error, error: ex);
            }
            finally
            {
                if (cancel.Token.IsCancellationRequested)
                {
                    OnSynchronizationChanged(SyncAction.Cancel);
                }

                syncWaiter.Set();

                lock (cancelLock)
                {
                    var existing = cancellationSource;
                    if (existing == cancel)
                    {
                        cancellationSource = null;
                    }
                }
            }
        }

        private async Task InternalSync(
                            TSynchronizationStages stagesToSynchronize,
            bool cancelExistingSynchronization)
        {
            try
            {
                lock (cancelLock)
                {
                    if (cancelExistingSynchronization)
                    {
                        var currentCancellation = cancellationSource;
                        if (currentCancellation != null)
                        {
                            currentCancellation.Cancel();
                        }
                    }
                }

                // Force the actual sync to a background thread to avoid blocking any UI
                await Task.Run(() => BackgroundThreadSync(stagesToSynchronize));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}