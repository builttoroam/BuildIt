using System.ComponentModel;

namespace BuildIt.States
{
    public interface IStateWithDataActionData<TState, TStateData,TData> :
        IStateWithDataAction<TState, TStateData>
        where TStateData : INotifyPropertyChanged
        where TState : struct
    {

    }
}