using System.Threading.Tasks;

namespace BuildIt.States.Interfaces
{
    public interface IAboutToLeave
    {
        Task AboutToLeave(CancelEventArgs cancel);
    }
}