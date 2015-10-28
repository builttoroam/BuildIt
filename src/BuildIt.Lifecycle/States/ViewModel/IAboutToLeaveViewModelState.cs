using System.ComponentModel;
using System.Threading.Tasks;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public interface IAboutToLeaveViewModelState
    {
        Task AboutToLeave(CancelEventArgs cancel);
    }
}