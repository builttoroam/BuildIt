using System;
using System.Collections.Generic;
using System.Text;
using BuildIt.States;

namespace BuildIt.forms.Sample.Core.ViewModels
{
    public enum SampleStates
    {
        Base,
        Show,
        Hide

    }

    public class MainViewModel:IHasStates
    {

        public IStateManager StateManager { get; } = new StateManager();

        public MainViewModel()
        {
            StateManager
                .Group<SampleStates>()
                .DefineAllStates();
        }

        private bool visible = true;
        public void SwitchStates()
        {
         visible = !visible;
            StateManager.GoToState(visible ? SampleStates.Show : SampleStates.Hide);
        }
    }
}
