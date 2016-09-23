using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.Synchronization
{
    public interface ISynchronizationContext<TSynchronizationStages> where TSynchronizationStages : struct
    {
        event EventHandler<SynchronizationEventArgs<TSynchronizationStages>> SynchronizationChanged;

        void DefineSynchronizationStep(TSynchronizationStages synchronizationStage,
            Func<ISynchronizationStage<TSynchronizationStages>, Task<bool>> stepAction);
        
        Task Synchronize(
            TSynchronizationStages stagesToSynchronize,
            bool cancelExistingSynchronization = false,
            bool waitForSynchronizationToComplete = false);

        Task Cancel(bool waitForSynchronizationToComplete = false);

    }

    public interface ISynchronizationStage<TStage> where TStage:struct
    {
        TStage Stage { get; }

        CancellationToken CancellationToken { get; set; }

        Action<ISynchronizationEventArgs> SynchronizationChanged { get; set; }

        Func<ISynchronizationStage<TStage>, Task<bool>> StepAction { get; }

        void RegisterSubStagesCount(int numberOfSubStages);

        void StartSubStage();

        void EndSubStage();

        void Progress(double progress);

        void RegisterSubStages<TSubStage>(params TSubStage[] stages) where TSubStage : struct;

        Task RunSubStage<TSubStage>(TSubStage stage, Func<ISynchronizationStage<TSubStage>, Task<bool>> stageAction)
            where TSubStage : struct;
    }
}