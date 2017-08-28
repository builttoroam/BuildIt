using System;

namespace BuildIt.Synchronization
{
    public interface INotifySynchronizationChanged<TSynchronizationStages>
        where TSynchronizationStages : struct
    {
        event EventHandler<SynchronizationEventArgs<TSynchronizationStages>> SynchronizationChanged;

        void OnSynchronizationChanged(
            SyncAction action,
            TSynchronizationStages? stage = null,
            ISynchronizationEventArgs child = null,
            double? percentageComplete = null,
            Exception error = null);
    }
}