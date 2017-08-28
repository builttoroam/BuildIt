using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.Synchronization
{
    public class SyncStepWrapper<TStage> : ISynchronizationStage<TStage>
        where TStage : struct
    {
        public static ISynchronizationStage<TSyncStage> Build<TSyncStage>(
            TSyncStage stage,
            Func<ISynchronizationStage<TSyncStage>, Task<bool>> action)
            where TSyncStage : struct
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
            get => subStageCount;
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

        public void OnSynchronizationChanged(
            SyncAction action,
            ISynchronizationEventArgs child = null,
            double? percentageComplete = null,
            Exception error = null)
        {
            var args = new SynchronizationEventArgs<TStage>
            {
                PercentageComplete = percentageComplete ?? 0,
                Stage = Stage,
                Action = action,
                Error = error,
                ChildStage = child
            };

            if (SynchronizationChanged == null)
            {
                return;
            }

            SynchronizationChanged(args);
        }

        public void StartSubStage()
        {
            CurrentSubStage++;
            var progSummary = SynchronizationEventArgs<TStage>.Build(SyncAction.Start, percentageComplete: 0.0);
            OnSynchronizationChanged(SyncAction.Progress, progSummary, StageIncrement * CurrentSubStage);
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

        public void RegisterSubStages<TSubStage>(params TSubStage[] stages)
            where TSubStage : struct
        {
            synchronizationSteps.AddRange(stages.OfType<object>());
            SubStageCount = synchronizationSteps.Count;
        }

        public async Task RunSubStage<TSubStage>(
            TSubStage stage,
            Func<ISynchronizationStage<TSubStage>, Task<bool>> stageAction)
            where TSubStage : struct
        {
            var progSummary = SynchronizationEventArgs<TSubStage>.Build(SyncAction.Start, stage, percentageComplete: 0.0);
            try
            {
                var idx = synchronizationSteps.IndexOf(stage);
                var progress = StageIncrement * idx;

                OnSynchronizationChanged(SyncAction.Progress, progSummary, progress);
                OnSynchronizationChanged(SyncAction.Progress, progSummary.Progress(SyncAction.Progress), progress);

                var subAction = SyncStepWrapper<TSubStage>.Build(
                    stage, stageAction);
                subAction.CancellationToken = CancellationToken;

                Action<ISynchronizationEventArgs> callback = percent =>
                {
                    var inc = StageIncrement * percent.PercentageComplete;
                    OnSynchronizationChanged(SyncAction.Progress, percent, progress + inc);
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

                OnSynchronizationChanged(SyncAction.Progress, progSummary.Progress(SyncAction.Progress, 1.0), progress + StageIncrement);
                OnSynchronizationChanged(SyncAction.Progress, progSummary.Progress(SyncAction.End, 1.0), progress + StageIncrement);
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