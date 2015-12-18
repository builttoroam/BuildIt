using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Autofac;
using BuildIt.Lifecycle;
using BuildIt.Lifecycle.States;
using BuildIt.Lifecycle.States.ViewModel;

namespace StateByState
{
    public enum SecondaryRegionView
    {
        Base,
        Main
    }
    public class SecondaryApplication : ApplicationRegion, IHasViewModelStateManager<SecondaryRegionView, MainRegionTransition>
    {
        public IViewModelStateManager<SecondaryRegionView, MainRegionTransition> StateManager { get; }

        public SecondaryApplication()
        {

            var sm = new ViewModelStateManager<SecondaryRegionView, MainRegionTransition>();

            sm.DefineViewModelState<SecondardyMainViewModel>(SecondaryRegionView.Main)
                .WhenChangedTo(vm =>
                {
                    vm.Done += State_Completed;
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
                    vm.Done -= State_Completed;
                });



            StateManager = sm;

        }


        public override  void RegisterDependencies(IContainer container)
        {
            (StateManager as ICanRegisterDependencies)?.RegisterDependencies(container);
        }


        private void State_Completed(object sender, EventArgs e)
        {
            OnCloseRegion();
        }

        protected async override Task CompleteStartup()
        {
            RegisterForUIAccess(StateManager);

            await base.CompleteStartup();

            await StateManager.ChangeTo(SecondaryRegionView.Main);
        }
    }
}