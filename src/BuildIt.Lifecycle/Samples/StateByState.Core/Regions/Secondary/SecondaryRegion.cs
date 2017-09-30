using BuildIt.Lifecycle.Regions;
using BuildIt.Lifecycle.States;
using BuildIt.States;
using BuildIt.States.Completion;
using System;
using System.Threading.Tasks;

namespace StateByState.Regions.Secondary
{
    public enum SecondaryRegionView
    {
        Base,
        Main
    }

    public class SecondaryRegion : StateAwareApplicationRegion
    {
        public SecondaryRegion()
        {
            StateManager.Group<SecondaryRegionView>()
                .DefineStateWithData<SecondaryRegionView, SecondaryMainViewModel>(SecondaryRegionView.Main)
                .OnComplete(DefaultCompletion.Complete)
                .CloseRegion(this);

            // .WhenChangedTo(vm =>
            // {
            //    vm.Done += State_Completed;
            //    Task.Run(async () =>
            //    {
            //        await Task.Delay(5000);
            //        await vm.UIContext.RunAsync(() =>
            //        {
            //            Debug.WriteLine("test");

            // });
            //    });
            // })
            // .WhenChangingFrom(vm =>
            // {
            //    vm.Done -= State_Completed;
            // });
        }

        // public override  void RegisterDependencies(IContainer container)
        // {
        //    (StateManager as ICanRegisterDependencies)?.RegisterDependencies(container);
        // }

        private void State_Completed(object sender, EventArgs e)
        {
            OnCloseRegion();
        }

        protected override async Task CompleteStartup()
        {
            // (StateManager as IRegisterForUIAccess)?.RegisterForUIAccess(this);

            await base.CompleteStartup();

            await StateManager.GoToState(SecondaryRegionView.Main);
        }
    }
}