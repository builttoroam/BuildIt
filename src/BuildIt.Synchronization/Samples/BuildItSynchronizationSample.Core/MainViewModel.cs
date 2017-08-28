using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


namespace BuildItSynchronizationSample.Core
{
    public class MainViewModel : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
