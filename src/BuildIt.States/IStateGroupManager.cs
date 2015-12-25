using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildIt.States
{
    public enum DefaultTransition
    {
        Base
    }

    public interface IStateGroupManager<TState, TTransition> : 
        //INotifyPropertyChanged, 
        IStateGroup
        where TState : struct
        where TTransition:struct
    {
        event EventHandler<StateEventArgs<TState>> StateChanged;

        TState CurrentState { get; }

        IDictionary<TState, IStateDefinition<TState>> States { get; }

        IDictionary<TTransition, ITransitionDefinition<TState>> Transitions { get; }

        void DefineAllStates();

        IStateDefinition<TState> DefineState(TState state);


        IStateDefinition<TState> DefineState(IStateDefinition<TState> stateDefinition);

        Task<bool> ChangeTo(TState newState, bool useTransition = true);

        Task<bool> Transition(TState newState, bool useTransition = true);

        Task<bool> Transition(TTransition transition, bool useTransition = true);
    }
}