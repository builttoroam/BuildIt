using System.Threading.Tasks;

namespace BuildIt.States
{
    public interface IStateGroup

    {
        Task<bool> ChangeTo<TFindState>(TFindState newState, bool useTransitions = true) where TFindState : struct;

        IStateBinder Bind(IStateGroup groupToBindTo);
    }

    public interface IStateBinder
    {
        void Unbind();
    }

    
}