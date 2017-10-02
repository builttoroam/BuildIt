using BuildIt;
using BuildIt.Lifecycle.Regions;
using BuildIt.States;
using System;
using System.Threading.Tasks;

namespace StateByState.Regions.Main.Third
{
    public class ThirdRegion : SingleAreaApplicationRegion<ThirdStates>
    {
        public ThirdRegion()
        {
            StateManager
                .Group<ThirdStates>()
                .DefineStateWithData<ThirdStates, ThirdOneViewModel>(ThirdStates.One)
                .DefineStateWithData<ThirdStates, ThirdTwoViewModel>(ThirdStates.Two)
                .DefineStateWithData<ThirdStates, ThirdThreViewModel>(ThirdStates.Three)
                .DefineStateWithData<ThirdStates, ThirdFourViewModel>(ThirdStates.Four)
                .WhenChangedTo((vm, cancel) =>
                {
                    vm.Done += Vm_Done;
                }).
                WhenChangingFrom((vm, cancel) =>
                {
                    vm.Done -= Vm_Done;
                });
        }

        public event EventHandler ThirdCompleted;

        protected override async Task CompleteStartup()
        {
            await base.CompleteStartup();

            await StateManager.GoToState(ThirdStates.One, false);
        }

        private void Vm_Done(object sender, EventArgs e)
        {
            ThirdCompleted.SafeRaise(this);
        }
    }
}