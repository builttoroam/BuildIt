using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BuildIt.States.UWP
{
    public static class UIStateHelpers
    {
        public static IStateGroupBuilder<TState> DefineAllStates<TState>(
   this IStateGroupBuilder<TState> vsmGroup, Control visualStatesRootElement, VisualStateGroup xamlVisualStateGroup)
   where TState : struct
        {
            if (vsmGroup == null) return null;
            vsmGroup.StateGroup.DefineAllStates();

            xamlVisualStateGroup.States.DoForEach(
                xamlVisualState =>
                {
                    var state = xamlVisualState.Name.EnumParse<TState>();
                    if (!state.Equals(default(TState)))
                    {
                        vsmGroup.DefineState(state).WhenChangedTo(() =>
                        {
                            var vstate = xamlVisualState;
                            if (xamlVisualStateGroup.CurrentState != vstate)
                            {
                                VisualStateManager.GoToState(visualStatesRootElement, vstate.Name, true);
                            }
                        });
                    }
                }
                );

            xamlVisualStateGroup.CurrentStateChanged += (s, e) =>
            {
                var state = e.NewState?.Name.EnumParse<TState>()??default(TState);
                vsmGroup.StateManager.GoToState(state);
            };

            return vsmGroup;
        }
    }
}
