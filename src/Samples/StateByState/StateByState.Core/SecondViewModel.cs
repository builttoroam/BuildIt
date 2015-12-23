using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BuildIt;
using BuildIt.Lifecycle.States;
using BuildIt.Lifecycle.States.ViewModel;
using BuildIt.Lifecycle;

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
    public class SecondViewModel : BaseViewModel, IHasStates
    {
        public event EventHandler SecondCompleted;


        public string Name { get; } = "Bob";

        public IStateManager StateManager { get; } = new StateManager();

        public SecondViewModel()
        {
            var gm = StateManager.GroupWithTransition<SecondStates, SecondStateTransitions>();
            gm.Item2.DefineAllStates();


            //StateManager = new BaseStateManager<SecondStates, SecondStateTransitions>();
            //StateManager.DefineAllStates();

            var gm2 = StateManager.GroupWithTransition<SecondStates2, SecondStateTransitions>();
            gm2.Item2.DefineAllStates();


            //StateManager2 = new BaseStateManager<SecondStates2, SecondStateTransitions>();
            //StateManager2.DefineAllStates();

        }

        public async Task InitSecond()
        {
           await  StateManager.GoToState(SecondStates.State1);
            await StateManager.GoToState(SecondStates2.StateX);
            await Task.Delay(1000);
            Debug.WriteLine("Break");
        }

        public void GoBack()
        {
            SecondCompleted?.Invoke(this, EventArgs.Empty);
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
            SecondCompleted?.Invoke(this, EventArgs.Empty);

        }

    }
}