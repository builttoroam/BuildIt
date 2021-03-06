using System.Diagnostics;
using Autofac;
using BuildIt;
using System;
using System.Threading.Tasks;
using BuildIt.Lifecycle;
using BuildIt.Lifecycle.Regions;
using BuildIt.Lifecycle.States;
using BuildIt.Lifecycle.States.ViewModel;
using BuildIt.States;
using BuildIt.States.Completion;

namespace StateByState
{
    public enum MainRegionView
    {
        Base,
        Main,
        Second,
        Third,
        Fourth
    }

    public enum MainRegionTransition
    {
        Base,
        MainToSecond,
        AnyToMain,
        ThirdToMain
    }

    public enum SimpleTest
    {
        Base,
        Test1,
        Test2
    }

    public class MainRegion : StateAwareApplicationRegion
    {

        public MainRegion()
        {

            #region State Definitions

            var group = StateManager.Group<SimpleTest>();


            StateManager.Group<MainRegionView>().WithHistory()
                .DefineStateWithData<MainRegionView, MainViewModel>(MainRegionView.Main)

                            .OnCompleteWithData(MainCompletion.Page2, vm => vm.TickCount)
                                .ChangeState(MainRegionView.Second)
                            .OnEvent((vm, a) => vm.UnableToComplete += a,
                                    (vm, a) => vm.UnableToComplete -= a)
                                .ChangeState(MainRegionView.Third)
                            .OnComplete(MainCompletion.NewRegion)
                                .LaunchRegion(this, TypeRef.Get<SecondaryApplication>())
                            .OnCompleteWithDataEvent<MainRegionView, MainViewModel, MainCompletion, int>(MainCompletion.Page4)
                                .ChangeState(MainRegionView.Fourth)
                                    .InitializeNewState<MainRegionView, MainViewModel, FourthViewModel, int>(
                                        (vm, d, cancel) => vm.InitValue = $"Started with {d}")
                            .Initialise(async (vm, cancel) =>
                            {
                                "VM State: Init".Log();
                                await vm.Init();
                            })
                            .WhenChangedTo(async (vm, cancel) =>
                            {
                                "VM State: When Changed To".Log();
                                //  vm.Completed += State_Completed;
                                //vm.UnableToComplete += State_UnableToCompleted;
                                //vm.SpawnNewRegion += Vm_SpawnNewRegion;
                                await vm.StartViewModel();
                            })
                            .WhenAboutToChange((vm, cancel) => $"VM State: About to Change - {cancel.Cancel}".Log())
                            .WhenChangingFrom(vm =>
                            {
                                "VM State: When Changing From".Log();
                                //vm.Completed -= State_Completed;
                                //vm.UnableToComplete -= State_UnableToCompleted;
                            })
                            //                        .AsState()
                            .WhenAboutToChangeFrom(cancel => $"State: About to Change - {cancel.Cancel}".Log())
#pragma warning disable 1998
                            .WhenChangingFrom(async (cancel) => "State: Changing".Log())
#pragma warning restore 1998
                            .WhenChangedTo((cancel) => Debug.WriteLine("State: Changing"))
                                    //                        .AsStateWithStateData<MainRegionView, MainViewModel>()
                                    //                        .EndState()

                                    .DefineStateWithData<MainRegionView, SecondViewModel>(MainRegionView.Second)
                                        .OnComplete(DefaultCompletion.Complete)
                                        .ChangeToPreviousState()
                                        .Initialise(async (vm, cancel) => await vm.InitSecond())
                                        .WhenChangedTo((vm, cancel) =>
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
                                        .WhenChangedToWithData<MainRegionView, SecondViewModel, int>((vm, data, cancel) =>
                                         {
                                             vm.ExtraData = data;
                                         })
                                        .WhenChangingFrom((vm, cancel) =>
                                        {
                                            //vm.SecondCompleted -= SecondCompleted;
                                        })
                                    //.EndState()
                                    .DefineStateWithData<MainRegionView, ThirdViewModel>(MainRegionView.Third)
                                        .WhenChangedTo((vm, cancel) =>
                                        {
                                            vm.ThirdCompleted += ThirdCompleted;
                                        })
                                        .WhenChangingFrom((vm, cancel) =>
                                        {
                                            vm.ThirdCompleted -= ThirdCompleted;
                                        });
            //.EndState();




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