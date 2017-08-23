using System;
using System.Threading.Tasks;
using Autofac;
using BuildIt;
using BuildIt.Lifecycle;
using BuildIt.Lifecycle.States;
using BuildIt.Lifecycle.States.ViewModel;
using BuildIt.States;
using StateByState.Services;

namespace StateByState
{
    public enum ThirdStates
    {
        Base,
        One,
        Two,
        Three,
        Four
    }

    public enum ThirdTransitions
    {
        Base,
        //OneToTwo,
        //TwoToThree,
        //ThreeToFour,
        //FourToOne
    }


    public class ThirdViewModel : BaseStateManagerViewModel
    {
        public event EventHandler ThirdCompleted;

        public ThirdViewModel()
        {

            StateManager.Group<ThirdStates>()
                .DefineStateWithData<ThirdStates,ThirdOneViewModel>(ThirdStates.One)
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

        //public override void RegisterDependencies(IContainer container)
        //{
        //    base.RegisterDependencies(container);

        //    (StateManager as ICanRegisterDependencies)?.RegisterDependencies(container);
        //}
        private void Vm_Done(object sender, EventArgs e)
        {
            ThirdCompleted.SafeRaise(this);
        }

        public async Task Start()
        {
            await StateManager.GoToState(ThirdStates.One);
        }

        public async void One()
        {
            await StateManager.GoToState(ThirdStates.Two);// ThirdTransitions.OneToTwo);
        }

        public async void Two()
        {
            await StateManager.GoToState(ThirdStates.Three);// ThirdTransitions.TwoToThree);

        }

        public async void Three()
        {

            await StateManager.GoToState(ThirdStates.Four);// ThirdTransitions.ThreeToFour);
        }
        public async void Four()
        {
            await StateManager.GoToState(ThirdStates.One);// ThirdTransitions.FourToOne);

        }
    }

    public class ThirdOneViewModel : NotifyBase { public string Title => "One"; }
    public class ThirdTwoViewModel : NotifyBase { public string Title => "Two"; }
    public class ThirdThreViewModel : NotifyBase { public string Title => "Three"; }

    public class ThirdFourViewModel : NotifyBase
    {
        public event EventHandler Done;
        public string Title { get; set; }

        public ThirdFourViewModel(ISpecial special)
        {
            Title = special.Data;
        }

        public void SayImDone()
        {
            Done.SafeRaise(this);
        }
    }
}
