using System.Reflection;
using System.Threading.Tasks;
using BuildIt.States;
using Xamarin.Forms;

namespace BuildIt.Forms.Core
{
    public static class StateManagerHelper
    {
        public static async Task GoToVisualState(this IStateManager manager, VisualState state)
        {
            //var gts = manager.GetType().GetRuntimeMethod("GoToState", new Type[] { state.StateType.GetType(),typeof(bool) });
            var gts = manager.GetType().GetTypeInfo().GetDeclaredMethod("GoToState");
            gts = gts.MakeGenericMethod(state.StateType.GetType());
            var stateChange = gts.Invoke(manager, new object[] {state.StateType, true}) as Task<bool>;
            var ok = await stateChange;
        }


        //public static IStateGroupBuilder<TState> DefineAllStates<TState>(
        //    this IStateGroupBuilder<TState> vsmGroup, BindableObject visualStatesRootElement,
        //    string xamlVisualStateGroupName)
        //    where TState : struct
        //{
        //    if (vsmGroup == null) return null;
        //    vsmGroup.StateGroup.DefineAllStates();


        //    var xamlVisualStateGroup = VisualStateManager.GetStateManager(visualStatesRootElement);
            
        //    xamlVisualStateGroup.States.DoForEach(
        //        xamlVisualState =>
        //        {
        //            var state = xamlVisualState.Name.EnumParse<TState>();
        //            if (!state.Equals(default(TState)))
        //            {
        //                vsmGroup.DefineState(state).WhenChangedTo(() =>
        //                {
        //                    var vstate = xamlVisualState;
        //                    if (xamlVisualStateGroup.CurrentState != vstate)
        //                    {
        //                        VisualStateManager.GoToState(visualStatesRootElement, vstate.Name, true);
        //                    }
        //                });
        //            }
        //        }
        //    );

        //    xamlVisualStateGroup.CurrentStateChanged += (s, e) =>
        //    {
        //        var state = e.NewState?.Name.EnumParse<TState>() ?? default(TState);
        //        vsmGroup.StateManager.GoToState(state);
        //    };

        //    return vsmGroup;
        //}


    }
}