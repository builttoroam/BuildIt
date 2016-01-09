using System.Diagnostics;
using Autofac;
using BuildIt;
using System;
using System.Threading.Tasks;
using BuildIt.Lifecycle;
using BuildIt.Lifecycle.States;
using BuildIt.Lifecycle.States.ViewModel;
using BuildIt.States;

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

    public class MainRegion : ApplicationRegion, IHasStates
    {
        public IStateManager StateManager { get; }


        public MainRegion()
        {

            #region State Definitions
            var ssm = new StateManager();
            var smx = ssm.GroupWithViewModels<MainRegionView>();
            var sm = smx.Item2;
                smx.StateWithViewModel<MainRegionView, MainViewModel>(MainRegionView.Main)
                .OnCompleteWithData(MainCompletion.Page2,vm=>vm.TickCount)
                .ChangeState(MainRegionView.Second)
                //.OnEvent((vm, a) => vm.Completed += a, 
                //        (vm, a) => vm.Completed -= a)
                //.ChangeState(MainRegionView.Second)
                .OnEvent((vm, a) => vm.UnableToComplete += a,
                        (vm, a) => vm.UnableToComplete -= a)
                .ChangeState(MainRegionView.Third)
                

                .Item3

            //sm.DefineViewModelState<MainViewModel>(MainRegionView.Main)
                .Initialise(async vm =>
                {
                    "VM State: Init".Log();
                    await vm.Init();
                })
                .WhenChangedTo(async vm =>
                {
                    "VM State: When Changed To".Log();
                  //  vm.Completed += State_Completed;
                    //vm.UnableToComplete += State_UnableToCompleted;
                    vm.SpawnNewRegion += Vm_SpawnNewRegion;
                    await vm.StartViewModel();
                })
                .WhenAboutToChange((vm, cancel) => $"VM State: About to Change - {cancel.Cancel}".Log())
                .WhenChangingFrom(vm =>
                {
                    "VM State: When Changing From".Log();
                    //vm.Completed -= State_Completed;
                    //vm.UnableToComplete -= State_UnableToCompleted;
                })
                .WhenAboutToChange(cancel => $"State: About to Change - {cancel.Cancel}".Log())
#pragma warning disable 1998
                .WhenChangingFrom(async () => "State: Changing".Log())
#pragma warning restore 1998
                .WhenChangedTo(() => Debug.WriteLine("State: Changing"));





            smx.StateWithViewModel<MainRegionView, SecondViewModel>(MainRegionView.Second)
                .OnComplete(DefaultCompletion.Complete)
                .ChangeToPreviousState()

                .Item3
            //sm.DefineViewModelState<SecondViewModel>(MainRegionView.Second)
                .Initialise(async vm => await vm.InitSecond())
                .WhenChangedTo(vm =>
                {
                    //vm.SecondCompleted += SecondCompleted;

                    Task.Run(async () =>
                    {
                        await Task.Delay(5000);
                        await vm.UIContext.RunAsync(() =>
                        {
                            Debug.WriteLine("test");

                        });
                    });
                })
                .WhenChangedToWithData<MainRegionView,SecondViewModel, int>((vm, data) =>
                {
                    vm.ExtraData = data;
                })
                .WhenChangingFrom(vm =>
                {
                    //vm.SecondCompleted -= SecondCompleted;
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


            StateManager = ssm;


            //sm.DefineTransition(MainRegionTransition.MainToSecond)
            //    .From(MainRegionView.Main)
            //    .To(MainRegionView.Second);

            //sm.DefineTransition(MainRegionTransition.AnyToMain)
            //    .To(MainRegionView.Main);

            //sm.DefineTransition(MainRegionTransition.ThirdToMain)
            //    .From(MainRegionView.Third)
            //    .To(MainRegionView.Main);
            #endregion

           
        }

        private void Vm_SpawnNewRegion(object sender, EventArgs e)
        {
            Manager.CreateRegion<SecondaryApplication>();
        }

        //public override void RegisterDependencies(IContainer container)
        //{
        //    base.RegisterDependencies(container);

        //    foreach (var stateGroup in StateManager.StateGroups)
        //    {
        //        (stateGroup.Value as ICanRegisterDependencies)?.RegisterDependencies(container);
        //    }

        //      //  (StateManager as ICanRegisterDependencies)?.RegisterDependencies(container);

        //    //RegionManager?.RegisterDependencies(container);
        //}

        //public override void RegisterForUIAccess(IUIExecutionContext context)
        //{
        //    base.RegisterForUIAccess(context);

        //    (StateManager as IRegisterForUIAccess)?.RegisterForUIAccess(this);
        //}

        protected override async Task CompleteStartup()
        {

            await base.CompleteStartup();

            await StateManager.GoToState(MainRegionView.Main, false);
        }


        //private async void State_Completed(object sender, EventArgs e)
        //{
        //    await StateManager.GoToState(MainRegionView.Second);
        //    // await StateManager.Transition(MainRegionTransition.MainToSecond);
        //}
        //private async void State_UnableToCompleted(object sender, EventArgs e)
        //{
        //    await StateManager.GoToState(MainRegionView.Third);
        //    //await StateManager.Transition(MainRegionView.Third);
        //}

        private async void SecondCompleted(object sender, EventArgs e)
        {
            await StateManager.GoBackToPreviousState();
            //await StateManager.GoToState(MainRegionView.Main);
            // await StateManager.Transition(MainRegionTransition.AnyToMain);
        }
        private async void ThirdCompleted(object sender, EventArgs e)
        {
            await StateManager.GoToState(MainRegionView.Main);
            // await StateManager.Transition(MainRegionTransition.ThirdToMain);
        }
    }
}