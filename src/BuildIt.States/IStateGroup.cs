using System;
using System.Threading.Tasks;

namespace BuildIt.States
{
    public interface IStateGroup

    {
        event EventHandler GoToPreviousStateIsBlockedChanged;

        Task<bool> ChangeTo<TFindState>(TFindState newState, bool useTransitions = true) where TFindState : struct;

        Task<bool> ChangeToWithData<TFindState,TData>(TFindState newState, TData data, bool useTransitions = true) where TFindState : struct;

        Task<bool> ChangeBackTo<TFindState>(TFindState newState, bool useTransitions = true) where TFindState : struct;

        Task<bool> ChangeToPrevious(bool useTransitions = true);

        IStateBinder Bind(IStateGroup groupToBindTo);

        bool TrackHistory { get; set; }

        bool HasHistory { get; }

        bool GoToPreviousStateIsBlocked { get; }
    }

    public interface IStateGroup<TState> : IStateGroup
        where TState : struct
    {
        IStateDefinition<TState> DefineState(IStateDefinition<TState> stateDefinition);
        IStateDefinition<TState> DefineState(TState state);
    }


    public interface IStateBinder
    {
        void Unbind();
    }

    
}