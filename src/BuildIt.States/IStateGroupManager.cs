using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildIt.States
{
    public enum DefaultTransition
    {
        Base
    }

    public interface IStateGroupManager<TState> :
        //INotifyPropertyChanged, 
        INotifyStateChanged<TState>,
        IStateGroup<TState>
        where TState : struct
    {
        TState CurrentState { get; }

        IDictionary<TState, IStateDefinition<TState>> States { get; }


        void DefineAllStates();

        //IStateDefinition<TState> DefineState(TState state);


        //IStateDefinition<TState> DefineState(IStateDefinition<TState> stateDefinition);

        Task<bool> ChangeTo(TState newState, bool useTransition = true);

    }
}