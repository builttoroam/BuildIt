using System.Threading.Tasks;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public interface ILeavingViewModelState
    {
        Task Leaving();
    }
}