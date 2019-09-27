using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BuildIt.Forms.Sample.Core.ViewModels
{
    public class StatefulControlViewModel : NotifyBase
    {
        public StatefulControlStates statefulControlState;

        private ICommand statefulControlPullToRefreshCommand;
        private ICommand statefulControlPullToRefreshRetryCommand;
        private ICommand statefulControlRetryCommand;

        public ObservableCollection<string> StatefulControlItems { get; } = new ObservableCollection<string>();

        public ICommand StatefulControlPullToRefreshCommand => statefulControlPullToRefreshCommand ?? (statefulControlPullToRefreshCommand = new Command(ExecuteStatefulControlPullToRefreshCommand));

        public ICommand StatefulControlPullToRefreshRetryCommand => statefulControlPullToRefreshRetryCommand ?? (statefulControlPullToRefreshRetryCommand = new Command(ExecuteStatefulControlPullToRefreshRetryCommand, () => StatefulControlState == StatefulControlStates.PullToRefreshError));

        public ICommand StatefulControlRetryCommand => statefulControlRetryCommand ?? (statefulControlRetryCommand = new Command(ExecuteStatefulControlRetryCommand, () => StatefulControlState == StatefulControlStates.LoadingError));

        public StatefulControlStates StatefulControlState
        {
            get => statefulControlState;
            set
            {
                if (SetProperty(ref statefulControlState, value))
                {
                    (StatefulControlRetryCommand as Command)?.ChangeCanExecute();
                    (StatefulControlPullToRefreshRetryCommand as Command)?.ChangeCanExecute();
                }
            }
        }

        public async Task UpdateStatefulControlState(StatefulControlStates newState, bool showLoadingIndicator = true)
        {
            // MK Imitate data loading
            if (showLoadingIndicator)
            {
                StatefulControlState = StatefulControlStates.Loading;
                await Task.Delay(2000);
            }

            if (newState != StatefulControlStates.PullToRefresh &&
                newState != StatefulControlStates.PullToRefreshError &&
                newState != StatefulControlStates.Loaded)
            {
                StatefulControlItems.Clear();
            }

            if (newState == StatefulControlStates.Loaded)
            {
                StatefulControlItems.Add("Bob");
                StatefulControlItems.Add("Adam");
                StatefulControlItems.Add("Nick");
                StatefulControlItems.Add("Andrew");
            }

            StatefulControlState = newState;
        }

        private async void ExecuteStatefulControlPullToRefreshCommand()
        {
            try
            {
                await Task.Delay(3000);
                var rand = new Random();
                if (rand.Next(3) % 2 == 0)
                {
                    throw new Exception("Every once in a while show an error state for the pull to refresh");
                }

                await UpdateStatefulControlState(StatefulControlStates.Loaded, false);
            }
            catch (Exception)
            {
                await UpdateStatefulControlState(StatefulControlStates.PullToRefreshError, false);
            }
        }

        private async void ExecuteStatefulControlPullToRefreshRetryCommand()
        {
            try
            {
                await UpdateStatefulControlState(StatefulControlStates.PullToRefresh, false);
                ExecuteStatefulControlPullToRefreshCommand();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async void ExecuteStatefulControlRetryCommand()
        {
            try
            {
                await UpdateStatefulControlState(StatefulControlStates.Loaded);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}