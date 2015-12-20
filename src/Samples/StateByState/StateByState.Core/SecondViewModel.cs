using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BuildIt;
using BuildIt.Lifecycle.States;
using BuildIt.Lifecycle.States.ViewModel;

namespace StateByState
{
    public enum SecondStates
    {
        Base,
        State1,
        State2,
        State3,
        State4
    }

    public enum SecondStateTransitions
    {
        Base,
        //State1To2,
        //State2To3,
        //State3To1,
        //StateXToZ,
        //StateYToZ,
        //StateZToY,
        //StateYToX
    }


    public enum SecondStates2
    {
        Base,
        StateX,
        StateY,
        StateZ
    }
    public class SecondViewModel : BaseViewModel
    {
        public event EventHandler SecondCompleted;


        public string Name { get; } = "Bob";

        public IStateManager<SecondStates, SecondStateTransitions> StateManager { get; }
        public IStateManager<SecondStates2, SecondStateTransitions> StateManager2 { get; }

        public SecondViewModel()
        {
            StateManager = new BaseStateManager<SecondStates, SecondStateTransitions>();
            StateManager.DefineAllStates();
            


            StateManager2 = new BaseStateManager<SecondStates2, SecondStateTransitions>();
            StateManager2.DefineAllStates();
            
        }

        public async Task InitSecond()
        {
           await  StateManager.ChangeTo(SecondStates.State1);
            await StateManager2.ChangeTo(SecondStates2.StateX);
            await Task.Delay(1000);
            Debug.WriteLine("Break");
        }

        public void GoBack()
        {
            SecondCompleted?.Invoke(this, EventArgs.Empty);
        }


        public void ToFirst()
        {
            StateManager.Transition(SecondStates.State1);
        }
        public void ToSecond()
        {
            StateManager.Transition(SecondStates.State2);
        }
        public void ToThird()
        {
            StateManager.Transition(SecondStates.State3);
        }


        public async void XtoZ()
        {
            await StateManager2.Transition(SecondStates2.StateZ);
        }
        public async void YtoZ()
        {
            await StateManager2.Transition(SecondStates2.StateZ);
        }
        public async void ZtoY()
        {
            await StateManager2.Transition(SecondStates2.StateY);
        }
        public async void YtoX()
        {
            await StateManager2.Transition(SecondStates2.StateX);
        }

        public void Done()
        {
            SecondCompleted?.Invoke(this, EventArgs.Empty);

        }

    }
}