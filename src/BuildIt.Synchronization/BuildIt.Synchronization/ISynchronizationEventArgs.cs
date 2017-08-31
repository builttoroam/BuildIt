using System;

namespace BuildIt.Synchronization
{
    public interface ISynchronizationEventArgs
    {
        SyncAction Action { get; set; }

        double PercentageComplete { get; set; }

        Exception Error { get; set; }

        ISynchronizationEventArgs ChildStage { get; set; }

        ISynchronizationEventArgs Progress(SyncAction action, double? percent = null, ISynchronizationEventArgs child = null, Exception error = null);
    }
}