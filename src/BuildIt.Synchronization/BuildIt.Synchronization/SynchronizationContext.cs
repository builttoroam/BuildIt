using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.Synchronization
{
    public enum SyncAction
    {
        None,
        Start,
        Progress,
        End,
        Error,
        Cancel
    }

    public class SynchronizationContext<TSynchronizationStages> :
        INotifySynchronizationChanged<TSynchronizationStages>,
        ISynchronizationContext<TSynchronizationStages>
        where TSynchronizationStages : struct
    {

        public event EventHandler<SynchronizationEventArgs<TSynchronizationStages>> SynchronizationChanged;

        public async Task Synchronize(TSynchronizationStages stagesToSynchronize,
            bool cancelExistingSynchronization = false,
            bool waitForSynchronizationToComplete = false)
        {
            var syncTask = InternalSync(stagesToSynchronize,
                cancelExistingSynchronization);
            if (waitForSynchronizationToComplete)
            {
                await syncTask;
            }
        }

        public async Task Cancel(bool waitForSynchronizationToComplete = false)
        {
            var syncTask = Task.Run(() => BackgroundThreadCancel());
            if (waitForSynchronizationToComplete)
            {
                await syncTask;
            }
        }

        public void OnSynchronizationChanged(SyncAction action,
            TSynchronizationStages? stage=null,
            ISynchronizationEventArgs child=null,
            double? percentageComplete = null,
            Exception error = null)
        {
            var args = new SynchronizationEventArgs<TSynchronizationStages>
            {
                PercentageComplete = percentageComplete??0,
                Stage = stage,
                Action = action,
                Error = error,
                ChildStage=child
            };

            if (SynchronizationChanged == null) return;
            SynchronizationChanged(this, args);
        }

        private readonly AutoResetEvent syncWaiter = new AutoResetEvent(true);

        private CancellationTokenSource cancellationSource;
        private readonly object cancelLock = new object();

        private async Task InternalSync(TSynchronizationStages stagesToSynchronize,
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
                    OnSynchronizationChanged(SyncAction.End, percentageComplete: 1.0);
                    return;
                }

                var percentage = 0.0;
                var increment = 1.0 / (double)steps.Length;
                OnSynchronizationChanged(SyncAction.Start, percentageComplete: 0.0);
                foreach (var step in steps)
                {
                    try
                    {
                        if (cancel.Token.IsCancellationRequested) return;

                        var progEvent = SynchronizationEventArgs<TSynchronizationStages>.Build(
                            SyncAction.Start,
                            step.Stage);

                        OnSynchronizationChanged(SyncAction.Progress, null, progEvent.Progress(SyncAction.Start,0.0), percentage);
                        OnSynchronizationChanged(SyncAction.Progress, null, progEvent.Progress(SyncAction.Progress), percentage);
                        Action<ISynchronizationEventArgs> callback = percent =>
                        {
                            var inc = increment * percent.PercentageComplete;
                            OnSynchronizationChanged(SyncAction.Progress,null, percent, percentage + inc);
                        };

                        step.CancellationToken = cancel.Token;
                        step.SynchronizationChanged = callback;
                        try
                        {
                            var ok = await step.StepAction(step); // InvokeStepAction(cancel.Token, callback);
                            if (cancel.Token.IsCancellationRequested) return;

                            if (!ok)
                            {
                                OnSynchronizationChanged(SyncAction.Error,null, progEvent.Progress(SyncAction.Error));
                                return;
                            }

                        }
                        catch(Exception ex)
                        {
                            OnSynchronizationChanged(SyncAction.Error, null, progEvent.Progress(SyncAction.Error,error: ex));
                            throw;
                        }
                        finally
                        {
                            // Make sure we clean up references to the token and callback
                            step.CancellationToken = default(CancellationToken);
                            step.SynchronizationChanged = null;
                        }


                        
                        percentage += increment;
                        OnSynchronizationChanged(SyncAction.Progress, null,progEvent.Progress(SyncAction.Progress,1.0), percentage);
                        OnSynchronizationChanged(SyncAction.Progress, null, progEvent.Progress(SyncAction.End), percentage);
                    }
                    catch (Exception ex)
                    {
                        OnSynchronizationChanged(SyncAction.Error, step.Stage, error: ex);
                        throw;
                    }
                }
                OnSynchronizationChanged(SyncAction.End, percentageComplete: 1.0);
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

        private readonly List<ISynchronizationStage<TSynchronizationStages>> synchronizationSteps = new List<ISynchronizationStage<TSynchronizationStages>>();

        public void DefineSynchronizationStep(TSynchronizationStages synchronizationStage, 
            Func<ISynchronizationStage<TSynchronizationStages>, Task<bool>> stepAction)
        {
            var wrapper = SyncStepWrapper<TSynchronizationStages>.Build(synchronizationStage,
                stepAction);
               
            synchronizationSteps.Add(wrapper);
        }
        
    }

    public interface INotifySynchronizationChanged<TSynchronizationStages> where TSynchronizationStages:struct
    {
        
        event EventHandler<SynchronizationEventArgs<TSynchronizationStages>> SynchronizationChanged;


        void OnSynchronizationChanged(SyncAction action,
            TSynchronizationStages? stage = null,
            ISynchronizationEventArgs child = null,
            double? percentageComplete = null,
            Exception error = null);
    }

    public class SyncStepWrapper<TStage> : ISynchronizationStage<TStage>
            where TStage : struct
    {

        public static ISynchronizationStage<TSyncStage> Build<TSyncStage>(TSyncStage stage,
            Func<ISynchronizationStage<TSyncStage>,Task<bool>> action
            ) where TSyncStage:struct
        {
            return new SyncStepWrapper<TSyncStage>
            {
                Stage = stage,
                StepAction = action
            };
        }

        public TStage Stage { get; private set; }
        public CancellationToken CancellationToken { get; set; }

        public Action<ISynchronizationEventArgs> SynchronizationChanged { get; set; }

        private double CurrentSubStage { get; set; }

        private double SubStageCount
        {
            get { return subStageCount; }
            set
            {
                subStageCount = value;
                StageIncrement = 1.0 / SubStageCount;
                CurrentSubStage = -1;

            }
        }

        private double StageIncrement { get; set; }

        public void RegisterSubStagesCount(int numberOfSubStages)
        {
            SubStageCount = numberOfSubStages;
        }


        public void OnSynchronizationChanged(SyncAction action,
        ISynchronizationEventArgs child = null,
        double? percentageComplete = null,
        Exception error = null)
        {
            var args = new SynchronizationEventArgs<TStage>
            {
                PercentageComplete = percentageComplete??0,
                Stage = Stage,
                Action = action,
                Error = error,
                ChildStage = child
            };

            if (SynchronizationChanged == null) return;
            SynchronizationChanged(args);
        }
       
        public void StartSubStage()
        {
            CurrentSubStage++;
            var progSummary = SynchronizationEventArgs<TStage>.Build(SyncAction.Start, percentageComplete: 0.0);
            OnSynchronizationChanged(SyncAction.Progress,progSummary, StageIncrement * CurrentSubStage);
        }

        public void EndSubStage()
        {
            var progSummary = SynchronizationEventArgs<TStage>.Build(SyncAction.End, percentageComplete: 1.0);
            OnSynchronizationChanged(SyncAction.Progress, progSummary, StageIncrement * (CurrentSubStage + 1));
        }

        public void Progress(double progress)
        {
            OnSynchronizationChanged(SyncAction.Progress, null, progress);
        }


        private readonly List<object> synchronizationSteps = new List<object>();
        private double subStageCount;


        public void RegisterSubStages<TSubStage>(params TSubStage[] stages) where TSubStage : struct
        {
            synchronizationSteps.AddRange(stages.OfType<object>());
            SubStageCount = synchronizationSteps.Count;
        }


        public async Task RunSubStage<TSubStage>(TSubStage stage,
            Func<ISynchronizationStage<TSubStage>, Task<bool>> stageAction) where TSubStage : struct
        {
            var progSummary = SynchronizationEventArgs<TSubStage>.Build(SyncAction.Start, stage, percentageComplete: 0.0);
            try
            {
                var idx = synchronizationSteps.IndexOf(stage);
                var progress = StageIncrement*idx;


                OnSynchronizationChanged(SyncAction.Progress, progSummary, progress);
                OnSynchronizationChanged(SyncAction.Progress, progSummary.Progress(SyncAction.Progress), progress);

                var subAction = SyncStepWrapper<TSubStage>.Build(
                    stage, stageAction);
                subAction.CancellationToken = CancellationToken;

                Action<ISynchronizationEventArgs> callback = percent =>
                {
                    var inc = StageIncrement * percent.PercentageComplete;
                    OnSynchronizationChanged(SyncAction.Progress,percent,  progress + inc);
                };
                subAction.SynchronizationChanged = callback;

                try
                {
                    await stageAction(subAction);
                }
                finally
                {
                    subAction.CancellationToken = default(CancellationToken);
                    subAction.SynchronizationChanged = null;
                }
                OnSynchronizationChanged(SyncAction.Progress, progSummary.Progress(SyncAction.Progress,1.0), progress + StageIncrement);
                OnSynchronizationChanged(SyncAction.Progress, progSummary.Progress(SyncAction.End,1.0), progress + StageIncrement);
            }
            catch (Exception ex)
            {
                // Swallow any step exception, but return false to indicate sync should abort;
                OnSynchronizationChanged(SyncAction.Progress, progSummary.Progress(SyncAction.Error));
                Debug.WriteLine(ex.Message);
            }
        }

        public Func<ISynchronizationStage<TStage>, Task<bool>> StepAction { get; set; }

    }
}
