using BuildIt.States.Interfaces.Builder;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BuildIt.States.UWP
{
    /// <summary>
    /// Helper methods for states
    /// </summary>
    public static class UIStateHelpers
    {
        /// <summary>
        /// Define all states based on visual state groups in XAML
        /// </summary>
        /// <typeparam name="TState">The type (enum) fo the state group</typeparam>
        /// <param name="vsmGroup">The state group builder</param>
        /// <param name="visualStatesRootElement">The visual root element</param>
        /// <param name="xamlVisualStateGroup">The visual state group</param>
        /// <returns>Builder for the state group</returns>
        public static IStateGroupBuilder<TState> DefineAllStates<TState>(
   this IStateGroupBuilder<TState> vsmGroup, Control visualStatesRootElement, VisualStateGroup xamlVisualStateGroup)
   where TState : struct
        {
            if (vsmGroup == null)
            {
                return null;
            }

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
                });

            xamlVisualStateGroup.CurrentStateChanged += (s, e) =>
            {
                var state = e.NewState?.Name.EnumParse<TState>() ?? default(TState);
                vsmGroup.StateManager.GoToState(state);
            };

            return vsmGroup;
        }
    }
}
