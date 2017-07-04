using BuildIt.States;
using BuildIt.States.Interfaces;

namespace BuildIt.forms.Sample.Core.ViewModels
{
    public class CustomViewModel:IHasStates
    {

        public IStateManager StateManager { get; } = new StateManager();

        public CustomViewModel()
        {
            StateManager
                .Group<SampleStates>()
                .DefineAllStates();
        }

        public void SwitchStates(bool visible)
        {
            StateManager.GoToState(visible ? SampleStates.Show : SampleStates.Hide);
        }
    }
}
