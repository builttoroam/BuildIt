using BuildIt;
using BuildIt.Lifecycle.Interfaces;
using BuildIt.Lifecycle.Regions;
using BuildIt.Lifecycle.States.ViewModel;
using BuildIt.ServiceLocation;
using BuildIt.States.Interfaces.StateData;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StateByState.Regions.Main.Third
{
    public class ThirdViewModel : BaseViewModel, IHasRegionManager, IInitialise
    {
        public ThirdViewModel()
        {
            RegionManager.DefineRegion<ThirdRegion>();
        }

        public event EventHandler ThirdCompleted;

        public IRegionManager RegionManager { get; } = new RegionManager();

        private ThirdRegion SubRegion { get; set; }

        public async void Four()
        {
            await SubRegion.StateManager.GoToState(ThirdStates.One);// ThirdTransitions.FourToOne);
        }

        public async Task Initialise(CancellationToken cancelToken)
        {
            SubRegion = RegionManager.CreateRegion<ThirdRegion>("ThirdRegion");
            await SubRegion.Startup(RegionManager);
            SubRegion.ThirdCompleted += SubRegion_ThirdCompleted;
        }

        private void SubRegion_ThirdCompleted(object sender, EventArgs e)
        {
            ThirdCompleted.SafeRaise(this);
        }

        public async void One()
        {
            await SubRegion.StateManager.GoToState(ThirdStates.Two);// ThirdTransitions.OneToTwo);
        }

        public override void RegisterDependencies(IDependencyContainer container)
        {
            base.RegisterDependencies(container);

            RegionManager.RegisterDependencies(container);
        }

        //public override void RegisterDependencies(IContainer container)
        // {
        //    base.RegisterDependencies(container);

        public async void Three()
        {
            await SubRegion.StateManager.GoToState(ThirdStates.Four);// ThirdTransitions.ThreeToFour);
        }

        //public async Task Start()
        //{
        //    await SubRegion.StateManager.GoToState(ThirdStates.One);
        //}
        public async void Two()
        {
            await SubRegion.StateManager.GoToState(ThirdStates.Three);// ThirdTransitions.TwoToThree);
        }

        // (StateManager as ICanRegisterDependencies)?.RegisterDependencies(container);
        // }
        //private void Vm_Done(object sender, EventArgs e)
        //{
        //    ThirdCompleted.SafeRaise(this);
        //}
    }
}