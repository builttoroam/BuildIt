using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BuildIt.States
{
    public interface IStateGroup:IRequiresUIAccess

    {
        event EventHandler GoToPreviousStateIsBlockedChanged;

        Task<bool> ChangeTo<TFindState>(TFindState newState, bool useTransitions = true) where TFindState : struct;

        Task<bool> ChangeToWithData<TFindState,TData>(TFindState newState, TData data, bool useTransitions = true) where TFindState : struct;

        Task<bool> ChangeBackTo<TFindState>(TFindState newState, bool useTransitions = true) where TFindState : struct;

        Task<bool> ChangeToPrevious(bool useTransitions = true);

        IStateBinder Bind(IStateGroup groupToBindTo, bool bothDirections=true);

        bool TrackHistory { get; set; }

        bool HasHistory { get; }

        bool GoToPreviousStateIsBlocked { get; }

        IDependencyContainer DependencyContainer { get; set; }

    }

    public interface IStateGroup<TState> : IStateGroup, INotifyStateChanged<TState>
        where TState : struct
    {


        IStateDefinition<TState> DefineState(IStateDefinition<TState> stateDefinition);
        IStateDefinition<TState> DefineState(TState state);

        IStateDefinitionWithData<TState, TStateData> DefineStateWithData<TStateData>(TState state)
            where TStateData : INotifyPropertyChanged;

        TState CurrentState { get; }

        IDictionary<TState, IStateDefinition<TState>> States { get; }


        void DefineAllStates();

        //IStateDefinition<TState> DefineState(TState state);


        //IStateDefinition<TState> DefineState(IStateDefinition<TState> stateDefinition);

        void WatchTrigger(IStateTrigger trigger);



    }


    public interface IStateBinder
    {
        void Unbind();
    }

    
}