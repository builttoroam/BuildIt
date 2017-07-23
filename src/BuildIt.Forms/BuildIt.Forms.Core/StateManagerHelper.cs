using System.Threading.Tasks;
using BuildIt.States.Interfaces;

namespace BuildIt.Forms
{
    public static class StateManagerHelper
    {
        public static async Task<bool> GoToVisualState(this IStateManager manager, VisualState state, bool animate=true)
        {
            return await manager.GoToState(state.StateGroup.GroupName,state.Name,animate);
        }
    }
}