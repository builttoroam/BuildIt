using System;
using System.Threading.Tasks;

namespace BuildIt.Synchronization
{
    public interface ISynchronizationContext<TSynchronizationStages>
        where TSynchronizationStages : struct
    {
        event EventHandler<SynchronizationEventArgs<TSynchronizationStages>> SynchronizationChanged;

        void DefineSynchronizationStep(
            TSynchronizationStages synchronizationStage,
            Func<ISynchronizationStage<TSynchronizationStages>, Task<bool>> stepAction);
        
        Task Synchronize(
            TSynchronizationStages stagesToSynchronize,
            bool cancelExistingSynchronization = false,
            bool waitForSynchronizationToComplete = false);

        Task Cancel(bool waitForSynchronizationToComplete = false);
    }
}