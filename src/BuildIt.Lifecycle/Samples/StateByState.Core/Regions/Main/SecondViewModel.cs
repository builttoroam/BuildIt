using BuildIt.Lifecycle.States.ViewModel;
using BuildIt.States;
using BuildIt.States.Completion;
using BuildIt.States.Interfaces.StateData;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace StateByState.Regions.Main
{
    public class SecondViewModel : BaseStateManagerViewModelWithCompletion<DefaultCompletion>,
        IChangedTo, IAboutToChangeFrom, IChangingFrom
    {
        private int extraData;
        private string name = "Bob";
        // public event EventHandler SecondCompleted;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public int ExtraData
        {
            get { return extraData; }
            set
            {
                extraData = value;
                OnPropertyChanged();
            }
        }

        public SecondViewModel()
        {
            StateManager.Group<SecondStates>().DefineAllStates();
            StateManager.Group<SecondStates2>().DefineAllStates();

            // StateManager = new BaseStateManager<SecondStates, SecondStateTransitions>();
            // StateManager.DefineAllStates();

            // var gm2 = StateManager.GroupWithTransition<SecondStates2, SecondStateTransitions>();
            // gm2.Item2.DefineAllStates();

            // StateManager2 = new BaseStateManager<SecondStates2, SecondStateTransitions>();
            // StateManager2.DefineAllStates();
        }

        public async Task InitSecond()
        {
            await StateManager.GoToState(SecondStates.State1);
            await StateManager.GoToState(SecondStates2.StateX);
            await Task.Delay(1000);
            Debug.WriteLine("Break");

            Run();
        }

        public async void Run()
        {
            for (int i = 0; i < 10; i++)
            {
                IsBlocked = !IsBlocked;
                await Task.Delay(1000);
            }
        }

        public void UpdateExtraData(int data)
        {
            ExtraData = data;
        }

        public void GoBack()
        {
            OnComplete(DefaultCompletion.Complete);
            // SecondCompleted?.Invoke(this, EventArgs.Empty);
        }

        public void ToFirst()
        {
            StateManager.GoToState(SecondStates.State1);
        }

        public void ToSecond()
        {
            StateManager.GoToState(SecondStates.State2);
        }

        public void ToThird()
        {
            StateManager.GoToState(SecondStates.State3);
        }

        public async void XtoZ()
        {
            await StateManager.GoToState(SecondStates2.StateZ);
        }

        public async void YtoZ()
        {
            await StateManager.GoToState(SecondStates2.StateZ);
        }

        public async void ZtoY()
        {
            await StateManager.GoToState(SecondStates2.StateY);
        }

        public async void YtoX()
        {
            await StateManager.GoToState(SecondStates2.StateX);
        }

        public void Done()
        {
            OnComplete(DefaultCompletion.Complete);
            // SecondCompleted?.Invoke(this, EventArgs.Empty);
        }

        public async Task ChangedTo(CancellationToken token)
        {
            await Task.Delay(2000);
            Name += ".... arrived ....";
        }

#pragma warning disable 1998 // sample only

        public async Task AboutToChangeFrom(StateCancelEventArgs cancel)
#pragma warning restore 1998
        {
            // cancel.Cancel = true;
        }

#pragma warning disable 1998 // sample only

        public async Task ChangingFrom(CancellationToken token)
#pragma warning restore 1998
        {
        }
    }
}