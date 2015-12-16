using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Autofac;
using BuildIt;
using BuildIt.Lifecycle;
using BuildIt.Lifecycle.States;
using BuildIt.Lifecycle.States.ViewModel;

namespace StateByState
{
    public enum PageStates
    {
        Base,
        Main,
        Second,
        Third
    }

    public enum PageTransitions
    {
        Base,
        MainToSecond,
        AnyToMain,
        ThirdToMain
    }



    public class SampleApplication : BaseApplication,IHasRegionManager
    {

        public IRegionManager RegionManager { get; }

        public SampleApplication()
        {

            RegionManager=new RegionManager();
            RegionManager.DefineRegion<MainWindow>();
            RegionManager.DefineRegion<SecondaryApplication>();
        }

        protected override async Task CompleteStartup()
        {
            await base.CompleteStartup();

            RegionManager.CreateRegion<MainWindow>();
        }

        protected override async Task BuildCoreDependencies(IContainer container)
        {
            await base.BuildCoreDependencies(container);

            RegionManager?.RegisterDependencies(container);
        }
    }

    public class MainWindow : ApplicationRegion, IHasViewModelStateManager<PageStates, PageTransitions>
    {
        public IViewModelStateManager<PageStates, PageTransitions> StateManager { get; }


        public MainWindow()
        {

            #region State Definitions
            var sm = new ViewModelStateManager<PageStates, PageTransitions>();

            sm.DefineViewModelState<MainViewModel>(PageStates.Main)
                .Initialise(async vm =>
                {
                    "VM State: Init".Log();
                    await vm.Init();
                })
                .WhenChangedTo(vm =>
                {
                    "VM State: When Changed To".Log();
                    vm.Completed += State_Completed;
                    vm.UnableToComplete += State_UnableToCompleted;
                    vm.SpawnNewRegion += Vm_SpawnNewRegion;
                })
                 .WhenAboutToChange((vm, cancel) => $"VM State: About to Change - {cancel.Cancel}".Log())
                 .WhenChangingFrom(vm =>
                 {
                     "VM State: When Changing From".Log();
                     vm.Completed -= State_Completed;
                     vm.UnableToComplete -= State_UnableToCompleted;
                 })
                .WhenAboutToChange(cancel => $"State: About to Change - {cancel.Cancel}".Log())
#pragma warning disable 1998
                .WhenChangingFrom(async () => "State: Changing".Log())
#pragma warning restore 1998
                .WhenChangedTo(() => Debug.WriteLine("State: Changing"));

            sm.DefineViewModelState<SecondViewModel>(PageStates.Second)
                .Initialise(async vm => await vm.InitSecond())
                .WhenChangedTo(vm =>
                {
                    vm.SecondCompleted += SecondCompleted;

                    Task.Run(async () =>
                    {
                        await Task.Delay(5000);
                        await vm.UIContext.RunAsync(() =>
                        {
                            Debug.WriteLine("test");

                        });
                    });
                })
                .WhenChangingFrom(vm =>
                {
                    vm.SecondCompleted -= SecondCompleted;
                });

            sm.DefineViewModelState<ThirdViewModel>(PageStates.Third)
                .WhenChangedTo(vm =>
                {
                    vm.ThirdCompleted += ThirdCompleted;
                })
                .WhenChangingFrom(vm =>
                {
                    vm.ThirdCompleted -= ThirdCompleted;
                });


            StateManager = sm;


            sm.DefineTransition(PageTransitions.MainToSecond)
                .From(PageStates.Main)
                .To(PageStates.Second);

            sm.DefineTransition(PageTransitions.AnyToMain)
                .To(PageStates.Main);

            sm.DefineTransition(PageTransitions.ThirdToMain)
                .From(PageStates.Third)
                .To(PageStates.Main);
            #endregion

           
        }

        private void Vm_SpawnNewRegion(object sender, EventArgs e)
        {
           Manager.CreateRegion<SecondaryApplication>();
        }

        public override void RegisterDependencies(IContainer container)
        {
            base.RegisterDependencies(container);

            (StateManager as ICanRegisterDependencies)?.RegisterDependencies(container);

            //RegionManager?.RegisterDependencies(container);
        }


        protected override async Task CompleteStartup()
        {
            RegisterForUIAccess(StateManager);

            await base.CompleteStartup();

            await StateManager.ChangeTo(PageStates.Main, false);
        }


        private async void State_Completed(object sender, EventArgs e)
        {
            await StateManager.Transition(PageTransitions.MainToSecond);
        }
        private async void State_UnableToCompleted(object sender, EventArgs e)
        {
            await StateManager.Transition(PageStates.Third);
        }

        private async void SecondCompleted(object sender, EventArgs e)
        {
            await StateManager.Transition(PageTransitions.AnyToMain);
        }
        private async void ThirdCompleted(object sender, EventArgs e)
        {
            await StateManager.Transition(PageTransitions.ThirdToMain);
        }
    }
}