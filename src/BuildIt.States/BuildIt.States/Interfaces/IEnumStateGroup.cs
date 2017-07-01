using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BuildIt.States.Interfaces
{
    public interface IEnumStateGroup<TState> : 
        IStateGroup, 
        INotifyEnumStateChanged<TState>,
        INotifyEnumStateChanging<TState>
        where TState : struct
    {


        IEnumStateDefinition<TState> DefineEnumState(IEnumStateDefinition<TState> stateDefinition);
        IEnumStateDefinition<TState> DefineEnumState(TState state);

        IEnumStateDefinitionWithData<TState, TStateData> DefineEnumStateWithData<TStateData>(TState state)
            where TStateData : INotifyPropertyChanged;

        TState CurrentEnumState { get; }

        IReadOnlyDictionary<TState, IEnumStateDefinition<TState>> EnumStates { get; }

        void DefineAllStates();

        Task<bool> ChangeTo(TState newState, bool useTransitions = true) ;

        Task<bool> ChangeToWithData<TData>(TState newState, TData data, bool useTransitions = true) ;

        Task<bool> ChangeBackTo(TState newState, bool useTransitions = true) ;


    }
}