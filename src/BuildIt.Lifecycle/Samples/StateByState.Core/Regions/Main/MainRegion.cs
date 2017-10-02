using BuildIt;
using BuildIt.Lifecycle.Regions;
using BuildIt.Lifecycle.States;
using BuildIt.States;
using BuildIt.States.Completion;
using StateByState.Regions.Main.Third;
using StateByState.Regions.Secondary;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace StateByState.Regions.Main
{
    public class MainRegion : SingleAreaApplicationRegion<MainRegionView>
    {
        public MainRegion()
        {
            StateManager
                .Group<MainRegionView>()
                .WithHistory()
                // *************** Main ***************
                .DefineStateWithData<MainRegionView, MainViewModel>(MainRegionView.Main)

                            .OnCompleteWithData(MainCompletion.Page2, vm => vm.TickCount)
                                .ChangeState(MainRegionView.Second)

                            .OnEvent((vm, a) => vm.UnableToComplete += a, (vm, a) => vm.UnableToComplete -= a)
                                .ChangeState(MainRegionView.Third)

                            .OnComplete(MainCompletion.NewRegion)
                                .LaunchRegion(this, TypeRef.Get<SecondaryRegion>())

                            .OnCompleteWithDataEvent<MainRegionView, MainViewModel, MainCompletion, int>(MainCompletion.Page4)
                                .ChangeState(MainRegionView.Fourth)
                                    .InitializeNewState<MainRegionView, MainViewModel, FourthViewModel, int>(
                                        (vm, d, cancel) => vm.InitValue = $"Started with {d}")

                            .Initialise(async (vm, cancel) =>
                            {
                                "VM State: Init".LogMessage();
                                await vm.Init();
                            })

                            .WhenChangedTo(async (vm, cancel) =>
                            {
                                "VM State: When Changed To".LogMessage();
                                vm.Completed += State_Completed;
                                // vm.UnableToComplete += State_UnableToCompleted;
                                // vm.SpawnNewRegion += Vm_SpawnNewRegion;
                                await vm.StartViewModel(cancel);
                            })

                            .WhenAboutToChange((vm, cancel) => $"VM State: About to Change - {cancel.Cancel}".LogMessage())

                            .WhenChangingFrom((vm, cancel) =>
                            {
                                "VM State: When Changing From".LogMessage();
                                vm.Completed -= State_Completed;
                                // vm.UnableToComplete -= State_UnableToCompleted;
                            })

                            .WhenAboutToChangeFrom(cancel => $"State: About to Change - {cancel.Cancel}".LogMessage())

#pragma warning disable 1998
                            .WhenChangingFrom(async (cancel) => "State: Changing".LogMessage())
#pragma warning restore 1998
                            .WhenChangedTo((cancel) => Debug.WriteLine("State: Changing"))

                // *************** Second ***************
                .DefineStateWithData<MainRegionView, SecondViewModel>(MainRegionView.Second)
                    .OnComplete(DefaultCompletion.Complete)
                        .ChangeToPreviousState()

                    .Initialise(async (vm, cancel) => await vm.InitSecond())

                    .WhenChangedTo((vm, cancel) =>
                    {
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
                        // vm.SecondCompleted -= SecondCompleted;
                    })

                // *************** Third ***************
                .DefineStateWithData<MainRegionView, ThirdViewModel>(MainRegionView.Third)
                    .WhenChangedTo((vm, cancel) =>
                    {
                        vm.ThirdCompleted += ThirdCompleted;
                    })
                    .WhenChangingFrom((vm, cancel) =>
                    {
                        vm.ThirdCompleted -= ThirdCompleted;
                    });
        }

        protected override async Task CompleteStartup()
        {
            await base.CompleteStartup();

            await StateManager.GoToState(MainRegionView.Main, false);
        }

        private async void State_Completed(object sender, EventArgs e)
        {
            await StateManager.GoToState(MainRegionView.Second);
        }

        // private async void State_UnableToCompleted(object sender, EventArgs e)
        // {
        //    await StateManager.GoToState(MainRegionView.Third);
        //    //await StateManager.Transition(MainRegionView.Third);
        // }

        private async void ThirdCompleted(object sender, EventArgs e)
        {
            await StateManager.GoToState(MainRegionView.Main);
        }
    }
}