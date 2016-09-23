using System.Threading.Tasks;

namespace BuildIt.Lifecycle.States.ViewModel
{
    public interface IArrivingViewModelState
    {
        Task Arriving();
    }
}