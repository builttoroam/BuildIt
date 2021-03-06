using BuildIt.Lifecycle;
using BuildIt.Lifecycle.States;
using BuildIt.States;
using BuildIt.States.Completion;
using System;
using System.Threading.Tasks;
using BuildIt.Lifecycle.Regions;

namespace StateByState
{
    public enum SecondaryRegionView
    {
        Base,
        Main
    }
    public class SecondaryApplication : StateAwareApplicationRegion
    {
        public SecondaryApplication()
        {

            StateManager.Group<SecondaryRegionView>()
                .DefineStateWithData<SecondaryRegionView, SecondardyMainViewModel>(SecondaryRegionView.Main)
                .OnComplete(DefaultCompletion.Complete)
                .CloseRegion(this);



            //.WhenChangedTo(vm =>
            //{
            //    vm.Done += State_Completed;
            //    Task.Run(async () =>
            //    {
            //        await Task.Delay(5000);
            //        await vm.UIContext.RunAsync(() =>
            //        {
            //            Debug.WriteLine("test");

            //        });
            //    });
            //})
            //.WhenChangingFrom(vm =>
            //{
            //    vm.Done -= State_Completed;
            //});

        }


        //public override  void RegisterDependencies(IContainer container)
        //{
        //    (StateManager as ICanRegisterDependencies)?.RegisterDependencies(container);
        //}


        private void State_Completed(object sender, EventArgs e)
        {
            OnCloseRegion();
        }

        protected override async Task CompleteStartup()
        {
            //(StateManager as IRegisterForUIAccess)?.RegisterForUIAccess(this);

            await base.CompleteStartup();

            await StateManager.GoToState(SecondaryRegionView.Main);
        }
    }
}