using System.Threading.Tasks;

namespace BuildIt.States.Interfaces.StateData
{
    public interface IAboutToChangeFrom
    {
        Task AboutToChangeFrom(CancelEventArgs cancel);
    }
}