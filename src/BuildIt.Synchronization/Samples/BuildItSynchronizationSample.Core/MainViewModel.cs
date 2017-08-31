using System;
using System.Threading.Tasks;
using BuildIt;

namespace BuildItSynchronizationSample.Core
{
    public class MainViewModel : NotifyBase
    {
        private SyncService syncService;

        public Action<Action> RunOnUIThreadAction { get; set; }

        private string progress;

        public string Progress
        {
            get => progress;

            set
            {
                if (progress != value)
                {
                    progress = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainViewModel()
        {
            syncService = new SyncService {ProgressAction = (s) =>
                {
                    RunOnUIThreadAction(() => { 
                        Progress = $"{Progress}\n{s}";
                    });
                }
            };
        }

        public async Task SynchroniseAll()
        {
            await syncService.SynchroniseAll(true);
        }

        public async Task SynchroniseStage1Only()
        {
            await syncService.SynchroniseStage1Only();
        }
    }
}
