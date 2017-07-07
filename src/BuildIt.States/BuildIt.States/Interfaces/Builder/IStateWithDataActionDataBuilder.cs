using System.ComponentModel;

namespace BuildIt.States.Interfaces.Builder
{
    /// <summary>
    /// Builder for state with state data that accepts data
    /// </summary>
    /// <typeparam name="TState">The type (enum) of the state</typeparam>
    /// <typeparam name="TStateData">The type of the state data</typeparam>
    /// <typeparam name="TData">The type of data</typeparam>
    public interface IStateWithDataActionDataBuilder<TState, TStateData, TData> :
        IStateWithDataActionBuilder<TState, TStateData>
        where TStateData : INotifyPropertyChanged
        where TState : struct
    {
    }
}