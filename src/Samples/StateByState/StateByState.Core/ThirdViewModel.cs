using System;
using System.Threading.Tasks;
using Autofac;
using BuildIt;
using BuildIt.Lifecycle.States;
using BuildIt.Lifecycle.States.ViewModel;
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

    public class ThirdViewModel:BaseViewModel, IHasViewModelStateManager<ThirdStates,ThirdTransitions>
    {
        public event EventHandler ThirdCompleted;

        public IViewModelStateManager<ThirdStates, ThirdTransitions> StateManager { get; }

        public ThirdViewModel()
        {

            var sm = new ViewModelStateManager<ThirdStates, ThirdTransitions>();
            sm.DefineViewModelState<ThirdOneViewModel>(ThirdStates.One);
            sm.DefineViewModelState<ThirdTwoViewModel>(ThirdStates.Two);
            sm.DefineViewModelState<ThirdThreViewModel>(ThirdStates.Three);
            sm.DefineViewModelState<ThirdFourViewModel>(ThirdStates.Four)
                .WhenChangedTo(vm =>
                {
                    vm.Done += Vm_Done;
                }).
                WhenChangingFrom(vm =>
                {
                    vm.Done -= Vm_Done;
                });
            StateManager = sm;
            
        }

        public override void RegisterDependencies(IContainer container)
        {
            base.RegisterDependencies(container);

            (StateManager as ICanRegisterDependencies)?.RegisterDependencies(container);
        }


        private void Vm_Done(object sender, EventArgs e)
        {
            ThirdCompleted.SafeRaise(this);
        }

        public async Task Start()
        {
            RegisterForUIAccess(StateManager);
            await StateManager.ChangeTo(ThirdStates.One);
        }

        public async Task One()
        {
            await StateManager.Transition(ThirdStates.Two);// ThirdTransitions.OneToTwo);
        }

        public async Task Two ()
        {
            await StateManager.Transition(ThirdStates.Three);// ThirdTransitions.TwoToThree);

        }

        public async Task Three()
        {

            await StateManager.Transition(ThirdStates.Four);// ThirdTransitions.ThreeToFour);
        }
        public async Task Four()
        {
            await StateManager.Transition(ThirdStates.One);// ThirdTransitions.FourToOne);

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
