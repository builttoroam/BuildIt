using System.Diagnostics;
using Autofac;
using BuildIt;
using System;
using System.Threading.Tasks;
using BuildIt.Lifecycle;
using BuildIt.Lifecycle.States;
using BuildIt.Lifecycle.States.ViewModel;

namespace StateByState
{
    public enum MainRegionView
    {
        Base,
        Main,
        Second,
        Third
    }

    public enum MainRegionTransition
    {
        Base,
        MainToSecond,
        AnyToMain,
        ThirdToMain
    }

    public class MainRegion : ApplicationRegion, 
        IHasViewModelStateManager<MainRegionView, MainRegionTransition>
    {
        public IViewModelStateManager<MainRegionView, MainRegionTransition> StateManager { get; }


        public MainRegion()
        {

            #region State Definitions
            var sm = new ViewModelStateManager<MainRegionView, MainRegionTransition>();

            sm.DefineViewModelState<MainViewModel>(MainRegionView.Main)
                .Initialise(async vm =>
                {
                    "VM State: Init".Log();
                    await vm.Init();
                })
                .WhenChangedTo(async vm =>
                {
                    "VM State: When Changed To".Log();
                    vm.Completed += State_Completed;
                    vm.UnableToComplete += State_UnableToCompleted;
                    vm.SpawnNewRegion += Vm_SpawnNewRegion;
                    await vm.StartViewModel();
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






            sm.DefineViewModelState<SecondViewModel>(MainRegionView.Second)
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

            sm.DefineViewModelState<ThirdViewModel>(MainRegionView.Third)
                .WhenChangedTo(vm =>
                {
                    vm.ThirdCompleted += ThirdCompleted;
                })
                .WhenChangingFrom(vm =>
                {
                    vm.ThirdCompleted -= ThirdCompleted;
                });


            StateManager = sm;


            sm.DefineTransition(MainRegionTransition.MainToSecond)
                .From(MainRegionView.Main)
                .To(MainRegionView.Second);

            sm.DefineTransition(MainRegionTransition.AnyToMain)
                .To(MainRegionView.Main);

            sm.DefineTransition(MainRegionTransition.ThirdToMain)
                .From(MainRegionView.Third)
                .To(MainRegionView.Main);
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

            await StateManager.ChangeTo(MainRegionView.Main, false);
        }


        private async void State_Completed(object sender, EventArgs e)
        {
            await StateManager.Transition(MainRegionTransition.MainToSecond);
        }
        private async void State_UnableToCompleted(object sender, EventArgs e)
        {
            await StateManager.Transition(MainRegionView.Third);
        }

        private async void SecondCompleted(object sender, EventArgs e)
        {
            await StateManager.Transition(MainRegionTransition.AnyToMain);
        }
        private async void ThirdCompleted(object sender, EventArgs e)
        {
            await StateManager.Transition(MainRegionTransition.ThirdToMain);
        }
    }
}