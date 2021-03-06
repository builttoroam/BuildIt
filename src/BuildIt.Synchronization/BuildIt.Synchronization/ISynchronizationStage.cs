using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.Synchronization
{
    public interface ISynchronizationStage<TStage>
        where TStage : struct
    {
        TStage Stage { get; }

        CancellationToken CancellationToken { get; set; }

        Action<ISynchronizationEventArgs> SynchronizationChanged { get; set; }

        Func<ISynchronizationStage<TStage>, Task<bool>> StepAction { get; }

        void RegisterSubStagesCount(int numberOfSubStages);

        void StartSubStage();

        void EndSubStage();

        void Progress(double progress);

        void RegisterSubStages<TSubStage>(params TSubStage[] stages)
            where TSubStage : struct;

        Task RunSubStage<TSubStage>(TSubStage stage, Func<ISynchronizationStage<TSubStage>, Task<bool>> stageAction)
            where TSubStage : struct;
    }
}