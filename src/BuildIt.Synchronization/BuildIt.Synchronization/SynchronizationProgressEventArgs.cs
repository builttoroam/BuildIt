using System;
using Microsoft;

namespace BuildIt.Synchronization
{
    public class SynchronizationEventArgs<TSynchronizationStages> : EventArgs,ISynchronizationEventArgs
        where TSynchronizationStages : struct
    {

        public static ISynchronizationEventArgs Build(SyncAction action,
            TSynchronizationStages? stage=null,
            ISynchronizationEventArgs child=null,
            double? percentageComplete = null,
            Exception error = null){

       
                var args = new SynchronizationEventArgs<TSynchronizationStages>
                {
                    PercentageComplete = percentageComplete??0.0,
                    Stage = stage,
                    Action = action,
                    Error = error,
                    ChildStage=child
                };

            return args;
            }

        public ISynchronizationEventArgs Progress(SyncAction action, double? percent = null, ISynchronizationEventArgs child = null, Exception error = null)
        {
            Action = action;
            if (percent.HasValue)
            {
                PercentageComplete = percent??0;
            }
            ChildStage = child;
            if (Error != null)
            {
                Error = error;
            }
            return this;
        }

        public SyncAction Action { get; set; }

        public TSynchronizationStages? Stage { get; set; }

        public double PercentageComplete { get; set; }

        public Exception Error { get; set; }

        public ISynchronizationEventArgs ChildStage { get; set; }

        public override string ToString()
        {
            var message = string.Empty;
            switch (Action)
            {
                case SyncAction.None:
                    message = "N/A";
                    break;
                case SyncAction.Start:
                    message = "Start";
                    break;
                case SyncAction.Progress:
                    message = "progress";
                    break;
                case SyncAction.End:
                    message = "End";
                    break;
                case SyncAction.Error:
                    message = "Error";
                    break;
                case SyncAction.Cancel:
                    message = "Cancelled";
                    break;
            }

            if (Stage.HasValue)
            {
                message += " " + Stage;
            }

                message += string.Format(" {0}%", PercentageComplete);

            if (Error != null)
            {
                message += " " + Error.Message;
            }

            if (ChildStage != null)
            {
                message += " [" + ChildStage + " ]";
            }

            if (!string.IsNullOrWhiteSpace(message)) return message;
            return base.ToString();
        }
    }


    public interface ISynchronizationEventArgs
    {
        SyncAction Action { get; set; }

        double PercentageComplete { get; set; }

        Exception Error { get; set; }

        ISynchronizationEventArgs ChildStage { get; set; }

        ISynchronizationEventArgs Progress(SyncAction action, double? percent = null, ISynchronizationEventArgs child = null, Exception error = null);

        
    }
}