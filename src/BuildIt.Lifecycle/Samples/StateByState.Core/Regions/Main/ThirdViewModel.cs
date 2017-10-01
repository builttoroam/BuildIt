using BuildIt;
using BuildIt.Lifecycle.Interfaces;
using BuildIt.Lifecycle.Regions;
using BuildIt.Lifecycle.States.ViewModel;
using BuildIt.ServiceLocation;
using BuildIt.States;
using BuildIt.States.Interfaces.StateData;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StateByState.Regions.Main
{
    public class ThirdViewModel : BaseViewModel, IHasRegionManager, IInitialise
    {
        public event EventHandler ThirdCompleted;

        public ThirdViewModel()
        {
            RegionManager.DefineRegion<ThirdRegion>();

            //StateManager.Group<ThirdStates>()
            //    .DefineStateWithData<ThirdStates, ThirdOneViewModel>(ThirdStates.One)
            //    .DefineStateWithData<ThirdStates, ThirdTwoViewModel>(ThirdStates.Two)
            //    .DefineStateWithData<ThirdStates, ThirdThreViewModel>(ThirdStates.Three)
            //    .DefineStateWithData<ThirdStates, ThirdFourViewModel>(ThirdStates.Four)
            //        .WhenChangedTo((vm, cancel) =>
            //        {
            //            vm.Done += Vm_Done;
            //        }).
            //        WhenChangingFrom((vm, cancel) =>
            //        {
            //            vm.Done -= Vm_Done;
            //        });
        }

        public IRegionManager RegionManager { get; } = new RegionManager();

        private IStateAwareApplicationRegion SubRegion { get; set; }

        public override void RegisterDependencies(IDependencyContainer container)
        {
            base.RegisterDependencies(container);

            RegionManager.RegisterDependencies(container);
        }

        //public override void RegisterDependencies(IContainer container)
        // {
        //    base.RegisterDependencies(container);

        // (StateManager as ICanRegisterDependencies)?.RegisterDependencies(container);
        // }
        private void Vm_Done(object sender, EventArgs e)
        {
            ThirdCompleted.SafeRaise(this);
        }

        //public async Task Start()
        //{
        //    await SubRegion.StateManager.GoToState(ThirdStates.One);
        //}

        public async void One()
        {
            await SubRegion.StateManager.GoToState(ThirdStates.Two);// ThirdTransitions.OneToTwo);
        }

        public async void Two()
        {
            await SubRegion.StateManager.GoToState(ThirdStates.Three);// ThirdTransitions.TwoToThree);
        }

        public async void Three()
        {
            await SubRegion.StateManager.GoToState(ThirdStates.Four);// ThirdTransitions.ThreeToFour);
        }

        public async void Four()
        {
            await SubRegion.StateManager.GoToState(ThirdStates.One);// ThirdTransitions.FourToOne);
        }

        public async Task Initialise(CancellationToken cancelToken)
        {
            SubRegion = RegionManager.CreateRegion<ThirdRegion>("ThirdRegion");
        }
    }

    public class ThirdRegion : SingleAreaApplicationRegion<ThirdStates>
    {
        public event EventHandler ThirdCompleted;

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

        private void Vm_Done(object sender, EventArgs e)
        {
            ThirdCompleted.SafeRaise(this);
        }

        protected override async Task CompleteStartup()
        {
            await base.CompleteStartup();

            await StateManager.GoToState(ThirdStates.One, false);
        }
    }
}