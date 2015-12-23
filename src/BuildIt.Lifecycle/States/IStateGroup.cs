using System.Threading.Tasks;

namespace BuildIt.Lifecycle.States
{
    public interface IStateGroup

    {
        Task<bool> ChangeTo<TFindState>(TFindState newState, bool useTransitions = true) where TFindState : struct;
    }
}