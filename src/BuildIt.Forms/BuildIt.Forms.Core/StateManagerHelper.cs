using BuildIt.States.Interfaces;
using System.Threading.Tasks;

namespace BuildIt.Forms
{
    /// <summary>
    /// Helper methods for working with a state manager.
    /// </summary>
    public static class StateManagerHelper
    {
        /// <summary>
        /// Invokes a state change to a new state.
        /// </summary>
        /// <param name="manager">The state manager.</param>
        /// <param name="state">The new state.</param>
        /// <param name="animate">Whether to animate or not.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task<bool> GoToVisualState(this IStateManager manager, VisualState state, bool animate = true)
        {
            return await manager.GoToState(state.StateGroup.GroupName, state.Name, animate);
        }
    }
}