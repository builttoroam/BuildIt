using BuildIt.Synchronization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using BuildIt;

namespace BuildItSynchronizationSample.Core
{
    public class SyncService 
    {
        [Flags]
        private enum SyncStages
        {
            None = 0,
            Stage1 = 1,
            Stage2 = 2,
            Stage3 = 4,
            All = Stage1 | Stage2 | Stage3
        }

        public Action<string> ProgressAction { get; set; }

        private ISynchronizationContext<SyncStages> SynchronizationManager { get; set; }

        public SyncService()
        {
            SynchronizationManager = new SynchronizationContext<SyncStages>();
            SynchronizationManager.DefineSynchronizationStep(SyncStages.Stage1, Stage1);
            SynchronizationManager.DefineSynchronizationStep(SyncStages.Stage2, Stage2);
            SynchronizationManager.DefineSynchronizationStep(SyncStages.Stage3, Stage3);
            SynchronizationManager.SynchronizationChanged += SynchronizationManager_SynchronizationProgressChanged;
        }

        public async Task SynchroniseAll(bool waitForCompletion)
        {
            await SynchronizationManager.Synchronize(SyncStages.All, waitForSynchronizationToComplete: waitForCompletion);
        }

        public async Task SynchroniseStage1Only()
        {
            await SynchronizationManager.Synchronize(SyncStages.Stage1, true, true);
        }

        public async Task Cancel()
        {
            await SynchronizationManager.Cancel(true);
        }

        private void SynchronizationManager_SynchronizationProgressChanged(object sender, SynchronizationEventArgs<SyncStages> e)
        {
            var message = e.ToString();
            ProgressAction(message);
        }

        private async Task<bool> Stage1(ISynchronizationStage<SyncStages> stage)
        {
            Debug.WriteLine("Stage 1");
            await Task.Delay(2000);
            return true;
        }

        private async Task<bool> Stage2(ISynchronizationStage<SyncStages> stage)
        {
            Debug.WriteLine("Stage 2");
            await Task.Delay(2000);

            return true;
        }

        private async Task<bool> Stage3(ISynchronizationStage<SyncStages> stage)
        {
            Debug.WriteLine("Stage 3");
            await Task.Delay(2000);
            return true;
        }
    }
}
